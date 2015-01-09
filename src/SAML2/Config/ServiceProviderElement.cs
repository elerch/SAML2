using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// ServiceProvider configuration element.
    /// </summary>
    public class ServiceProviderElement 
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string Server { get; set; }


        /// <summary>
        /// Gets or sets the authentication contexts.
        /// </summary>
        /// <value>The authentication contexts.</value>
        public AuthenticationContextCollection AuthenticationContexts
        { get; set; }

        /// <summary>
        /// Gets or sets the endpoints.
        /// </summary>
        /// <value>The endpoints.</value>
        public ServiceProviderEndpointCollection Endpoints
        {
            get { return (ServiceProviderEndpointCollection)base["endpoints"]; }
            set { base["endpoints"] = value; }
        }

        /// <summary>
        /// Gets or sets the name id formats.
        /// </summary>
        /// <value>The name id formats.</value>
        public NameIdFormatCollection NameIdFormats
        {
            get { return (NameIdFormatCollection)base["nameIdFormats"]; }
            set { base["nameIdFormats"] = value; }
        }

        /// <summary>
        /// Gets or sets the signing certificate.
        /// </summary>
        /// <value>The signing certificate.</value>
        public CertificateElement SigningCertificate
        {
            get { return (CertificateElement)base["signingCertificate"]; }
            set { base["signingCertificate"] = value; }
        }

    }
}
