using System;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Encapsulates the ArtifactResponse schema class
    /// </summary>
    public class Saml20ArtifactResponse
    {
        /// <summary>
        /// The artifact response.
        /// </summary>
        private readonly ArtifactResponse _artifactResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20ArtifactResponse"/> class.
        /// </summary>
        public Saml20ArtifactResponse()
        {
            _artifactResponse = new ArtifactResponse
                                    {
                                        Version = Saml20Constants.Version,
                                        ID = "id" + Guid.NewGuid().ToString("N"),
                                        Issuer = new NameId(),
                                        IssueInstant = DateTime.Now,
                                        Status = new Status { StatusCode = new StatusCode() }
                                    };
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public string Id
        {
            get { return _artifactResponse.ID; }
        }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get { return _artifactResponse.Issuer.Value; }
            set { _artifactResponse.Issuer.Value = value; }
        }

        /// <summary>
        /// Gets or sets InResponseTo.
        /// </summary>
        /// <value>The in response to.</value>
        public string InResponseTo
        {
            get { return _artifactResponse.InResponseTo; }
            set { _artifactResponse.InResponseTo = value; }
        }

        /// <summary>
        /// Gets or sets the SAML element.
        /// </summary>
        /// <value>The SAML element.</value>
        public XmlElement SamlElement
        {
            get { return _artifactResponse.Any;  }
            set { _artifactResponse.Any = value;  }
        }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public string StatusCode
        {
            get { return _artifactResponse.Status.StatusCode.Value; }
            set { _artifactResponse.Status.StatusCode.Value = value; }
        }

        /// <summary>
        /// Gets a default instance of this class with proper values set.
        /// </summary>
        /// <returns>The default <see cref="Saml20ArtifactResponse"/>.</returns>
        public static Saml20ArtifactResponse GetDefault()
        {
            var result = new Saml20ArtifactResponse();
            var config = Saml2Config.GetConfig();
            result.Issuer = config.ServiceProvider.Id;

            return result;
        }

        /// <summary>
        /// Returns the ArtifactResponse as an XML document.
        /// </summary>
        /// <returns>The XML document.</returns>
        public XmlDocument GetXml()
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(Serialization.SerializeToXmlString(_artifactResponse));

            return doc;
        }
    }
}
