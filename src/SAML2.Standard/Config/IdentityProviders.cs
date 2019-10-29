using System;
using System.Collections.Generic;
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
    //[ConfigurationCollection(typeof(IdentityProviderElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class IdentityProviders : List<IdentityProvider>
    {
        /// <summary>
        /// A list of the files that have currently been loaded. The filename is used as key, while last seen modification time is used as value.
        /// </summary>
        private Dictionary<string, DateTime> _fileInfo;

        /// <summary>
        /// The locking object for assuring thread safe refresh.
        /// </summary>
        private object _lockSync = new object();

        public IdentityProviders() : base() { Initialize(); }

        public IdentityProviders(IEnumerable<IdentityProvider> collection) : base(collection) { Initialize(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityProviders"/> class.
        /// </summary>
        private void Initialize()
        {
            _fileInfo = new Dictionary<string, DateTime>();
        }



        /// <summary>
        /// Gets or sets the encodings.
        /// </summary>
        public string Encodings { get; set; }


        /// <summary>
        /// Gets the selection URL to use for choosing identity providers if multiple are available and none are set as default.
        /// </summary>
        public string SelectionUrl { get; set; }

        public void AddByMetadataUrl(Uri url)
        {
            var request = System.Net.WebRequest.Create(url);
            // It may be more efficient to pass the stream directly, but
            // it's likely a bit safer to pull the data off the response
            // stream and create a new memorystream with the data
            using (var ms = new MemoryStream()) {
                using (var response = request.GetResponse().GetResponseStream()) {
                    response.CopyTo(ms);
                    response.Close();
                }
                ms.Seek(0, SeekOrigin.Begin); // Rewind memorystream back to the beginning
                // We want to allow exceptions to bubble up in this case
                var metadataDoc = new Saml20MetadataDocument(ms, GetEncodings());
                AdjustIdpListWithNewMetadata(metadataDoc);
            }
        }

        public void AddByMetadataDirectory(string path)
        {
            AddByMetadata(Directory.GetFiles(path));
        }

        public void AddByMetadata(params string[] files)
        {
            foreach (var file in files) {
                TryAddByMetadata(file); // ignore errors
            }
        }
        public bool TryAddByMetadata(string file)
        {
            try {
                var metadataDoc = new Saml20MetadataDocument(file, GetEncodings());
                AdjustIdpListWithNewMetadata(metadataDoc);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        private void AdjustIdpListWithNewMetadata(Saml20MetadataDocument metadataDoc)
        {
            var endp = this.FirstOrDefault(x => x.Id == metadataDoc.EntityId);
            if (endp == null) {
                // If the endpoint does not exist, create it.
                endp = new IdentityProvider();
                Add(endp);
            }

            endp.Id = endp.Name = metadataDoc.EntityId;
            endp.Metadata = metadataDoc;
        }


        /// <summary>
        /// Returns a list of the encodings that should be tried when a metadata file does not contain a valid signature
        /// or cannot be loaded by the XmlDocument class. Either returns a list specified by the administrator in the configuration file
        /// or a default list.
        /// </summary>
        /// <returns>The list of encodings.</returns>
        internal IEnumerable<Encoding> GetEncodings()
        {
            var rc = string.IsNullOrEmpty(Encodings)
                                  ? new [] { Encoding.UTF8, Encoding.GetEncoding("iso-8859-1") }
                                  : Encodings.Split(' ').Select(Encoding.GetEncoding);

            return rc;
        }
    }
}
