using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using SAML2.Schema.Metadata;
using SAML2.Utils;

namespace SAML2.Config
{
    /// <summary>
    /// Identity Provider configuration collection.
    /// </summary>
    [ConfigurationCollection(typeof(IdentityProviderElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class IdentityProviderCollection : EnumerableConfigurationElementCollection<IdentityProviderElement>
    {
        /// <summary>
        /// The file system watcher.
        /// </summary>
        private readonly FileSystemWatcher _fileSystemWatcher;

        /// <summary>
        /// Contains Encoding instances of the the encodings that should by tried when a metadata file does not have its
        /// encoding specified.
        /// </summary>
        private List<Encoding> _encodings;

        /// <summary>
        /// A list of the files that have currently been loaded. The filename is used as key, while last seen modification time is used as value.
        /// </summary>
        private Dictionary<string, DateTime> _fileInfo;

        /// <summary>
        /// This dictionary links a file name to the entity id of the metadata document in the file.
        /// </summary>
        private Dictionary<string, string> _fileToEntity;

        /// <summary>
        /// The locking object for assuring thread safe refresh.
        /// </summary>
        private object _lockSync = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityProviderCollection"/> class.
        /// </summary>
        public IdentityProviderCollection()
        {
            _fileInfo = new Dictionary<string, DateTime>();
            _fileToEntity = new Dictionary<string, string>();

            _fileSystemWatcher = new FileSystemWatcher
                                     {
                                         Filter = "*.*",
                                         NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                                         Path = MetadataLocation,
                                         EnableRaisingEvents = true
                                     };

            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Created += FileSystemWatcher_Changed;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Changed;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
        }

        #region Attributes

        /// <summary>
        /// Gets or sets the encodings.
        /// </summary>
        [ConfigurationProperty("encodings")]
        public string Encodings
        {
            get { return (string)base["encodings"]; }
            set { base["encodings"] = value; }
        }

        /// <summary>
        /// Gets or sets the metadata location.
        /// </summary>
        [ConfigurationProperty("metadata")]
        public string MetadataLocation
        {
            get
            {
                var value = (string)base["metadata"];
                if (!Path.IsPathRooted(value))
                {
                    return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
                }

                return value;
            }

            set
            {
                base["metadata"] = value;
                _fileSystemWatcher.Path = MetadataLocation;
                Refresh();
            }
        }

        /// <summary>
        /// Gets the selection URL to use for choosing identity providers if multiple are available and none are set as default.
        /// </summary>
        [ConfigurationProperty("selectionUrl")]
        public string SelectionUrl
        {
            get { return (string)base["selectionUrl"]; }
        }

        #endregion

        /// <summary>
        /// Refreshes this instance from metadata location.
        /// </summary>
        public void Refresh()
        {
            if (MetadataLocation == null)
            {
                return;
            }

            if (!Directory.Exists(MetadataLocation))
            {
                throw new DirectoryNotFoundException(string.Format(ErrorMessages.MetadataLocationNotFound, MetadataLocation));
            }

            // Start by removing information on files that are no long in the directory.
            var keys = new List<string>(_fileInfo.Keys.Count);
            keys.AddRange(_fileInfo.Keys);
            foreach (string file in keys)
            {
                if (!File.Exists(file))
                {
                    _fileInfo.Remove(file);
                    if (_fileToEntity.ContainsKey(file))
                    {
                        var endp = this.FirstOrDefault(x => x.Id == _fileToEntity[file]);
                        if (endp != null)
                        {
                            endp.Metadata = null;
                        }

                        _fileToEntity.Remove(file);
                    }
                }
            }

            // Detect added classes
            var files = Directory.GetFiles(MetadataLocation);
            foreach (var file in files)
            {
                Saml20MetadataDocument metadataDoc;
                if (_fileInfo.ContainsKey(file) && _fileInfo[file] == File.GetLastWriteTime(file))
                {
                    continue;
                }

                metadataDoc = ParseFile(file);

                if (metadataDoc != null)
                {
                    var endp = this.FirstOrDefault(x => x.Id == metadataDoc.EntityId);
                    if (endp == null)
                    {
                        // If the endpoint does not exist, create it.
                        endp = new IdentityProviderElement();
                        BaseAdd(endp);
                    }

                    endp.Id = endp.Name = metadataDoc.EntityId;
                    endp.Metadata = metadataDoc;

                    if (_fileToEntity.ContainsKey(file))
                    {
                        _fileToEntity.Remove(file);
                    }

                    _fileToEntity.Add(file, metadataDoc.EntityId);
                }
            }
        }

        /// <summary>
        /// Parses the geneva server metadata.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns>The XML document.</returns>
        private static XmlDocument ParseGenevaServerMetadata(XmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            if (doc.DocumentElement == null)
            {
                throw new ArgumentException("DocumentElement cannot be null", "doc");
            }

            var other = new XmlDocument { PreserveWhitespace = true };
            other.LoadXml(doc.OuterXml);

            foreach (var node in other.DocumentElement.ChildNodes.Cast<XmlNode>().Where(node => node.Name != IdpSsoDescriptor.ElementName).ToList())
            {
                other.DocumentElement.RemoveChild(node);
            }

            return other;
        }

        /// <summary>
        /// Handles the Renamed event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (_lockSync)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Handles the Changed event of the FileSystemWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (_lockSync)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Returns a list of the encodings that should be tried when a metadata file does not contain a valid signature
        /// or cannot be loaded by the XmlDocument class. Either returns a list specified by the administrator in the configuration file
        /// or a default list.
        /// </summary>
        /// <returns>The list of encodings.</returns>
        private List<Encoding> GetEncodings()
        {
            if (_encodings != null)
            {
                return _encodings;
            }

            _encodings = string.IsNullOrEmpty(Encodings)
                                  ? new List<Encoding> { Encoding.UTF8, Encoding.GetEncoding("iso-8859-1") }
                                  : new List<Encoding>(Encodings.Split(' ').Select(Encoding.GetEncoding));

            return _encodings;
        }

        /// <summary>
        /// Loads a file into an XmlDocument. If the loading or the signature check fails, the method will retry using another encoding.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The XML document.</returns>
        private XmlDocument LoadFileAsXmlDocument(string filename)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };

            try
            {
                // First attempt a standard load, where the XML document is expected to declare its encoding by itself.
                doc.Load(filename);
                try
                {
                    if (XmlSignatureUtils.IsSigned(doc) && !XmlSignatureUtils.CheckSignature(doc))
                    {
                        // Bad, bad, bad... never use exceptions for control flow! Who wrote this?
                        // Throw an exception to get into quirksmode.
                        throw new InvalidOperationException("Invalid file signature");
                    }
                }
                catch (CryptographicException)
                {
                    // Ignore cryptographic exception caused by Geneva server's inability to generate a
                    // .NET compliant xml signature
                    return ParseGenevaServerMetadata(doc);
                }

                return doc;
            }
            catch (XmlException)
            {
                // Enter quirksmode
                foreach (var encoding in GetEncodings())
                {
                    StreamReader reader = null;
                    try
                    {
                        reader = new StreamReader(filename, encoding);
                        doc.Load(reader);
                        if (XmlSignatureUtils.IsSigned(doc) && !XmlSignatureUtils.CheckSignature(doc))
                        {
                            continue;
                        }
                    }
                    catch (XmlException)
                    {
                        continue;
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }

                    return doc;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses the metadata files found in the directory specified in the configuration.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The parsed <see cref="Saml20MetadataDocument"/>.</returns>
        private Saml20MetadataDocument ParseFile(string file)
        {
            var doc = LoadFileAsXmlDocument(file);

            try
            {
                foreach (var child in doc.ChildNodes.Cast<XmlNode>().Where(child => child.NamespaceURI == Saml20Constants.Metadata))
                {
                    if (child.LocalName == EntityDescriptor.ElementName)
                    {
                        return new Saml20MetadataDocument(doc);
                    }

                    // TODO Decide how to handle several entities in one metadata file.
                    if (child.LocalName == EntitiesDescriptor.ElementName)
                    {
                        throw new NotImplementedException();
                    }
                }

                // No entity descriptor found. 
                throw new InvalidDataException();
            }
            catch (Exception e)
            {
                // Probably not a metadata file.
                Logging.LoggerProvider.LoggerFor(GetType()).Error("Problem parsing metadata file", e);
                return null;
            }
        }
    }
}
