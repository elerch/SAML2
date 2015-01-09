using System;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Encapsulates the ArtifactResolve schema class.
    /// </summary>
    public class Saml20ArtifactResolve
    {
        /// <summary>
        /// Artifact resolve.
        /// </summary>
        private readonly ArtifactResolve _artifactResolve;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20ArtifactResolve"/> class.
        /// </summary>
        public Saml20ArtifactResolve()
        {
            _artifactResolve = new ArtifactResolve
                                   {
                                       Version = Saml20Constants.Version,
                                       Id = "id" + Guid.NewGuid().ToString("N"),
                                       Issuer = new NameId(),
                                       IssueInstant = DateTime.Now
                                   };
        }

        /// <summary>
        /// Gets or sets the artifact string.
        /// </summary>
        /// <value>The artifact string.</value>
        public string Artifact
        {
            get { return _artifactResolve.Artifact; }
            set { _artifactResolve.Artifact = value; }
        }
        
        /// <summary>
        /// Gets the ID of the SAML message.
        /// </summary>
        /// <value>The ID.</value>
        public string Id
        {
            get { return _artifactResolve.Id; }
        }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get { return _artifactResolve.Issuer.Value; }
            set { _artifactResolve.Issuer.Value = value; }
        }

        /// <summary>
        /// Gets the underlying schema instance.
        /// </summary>
        /// <value>The <see cref="ArtifactResolve"/>.</value>
        public ArtifactResolve Resolve
        {
            get
            {
                return _artifactResolve;
            }
        }

        /// <summary>
        /// Gets a default instance of this class with proper values set.
        /// </summary>
        /// <returns>The default <see cref="Saml20ArtifactResolve"/>.</returns>
        public static Saml20ArtifactResolve GetDefault()
        {
            var config = Saml2Config.GetConfig();
            var result = new Saml20ArtifactResolve { Issuer = config.ServiceProvider.Id };

            return result;
        }

        /// <summary>
        /// Returns the ArtifactResolve as an XML document.
        /// </summary>
        /// <returns>The XML document.</returns>
        public XmlDocument GetXml()
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(Serialization.SerializeToXmlString(_artifactResolve));

            return doc;
        }
    }
}
