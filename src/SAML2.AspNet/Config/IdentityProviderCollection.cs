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

namespace SAML2.AspNet.Config
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
            // Is this really absolutely necessary? Are people changing metadata at runtime?
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

                metadataDoc = SAML2.Config.IdentityProviders.ParseFile(file, GetEncodings);

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

    }
}
