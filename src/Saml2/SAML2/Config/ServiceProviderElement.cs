using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// ServiceProvider configuration element.
    /// </summary>
    public class ServiceProviderElement : WritableConfigurationElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
            set { base["id"] = value; }
        }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        [ConfigurationProperty("server", IsRequired = true)]
        public string Server
        {
            get { return (string)base["server"]; }
            set { base["server"] = value; }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the authentication contexts.
        /// </summary>
        /// <value>The authentication contexts.</value>
        [ConfigurationProperty("authenticationContexts")]
        public AuthenticationContextCollection AuthenticationContexts
        {
            get { return (AuthenticationContextCollection)base["authenticationContexts"]; }
            set { base["authenticationContexts"] = value; }
        }

        /// <summary>
        /// Gets or sets the endpoints.
        /// </summary>
        /// <value>The endpoints.</value>
        [ConfigurationProperty("endpoints", Options = ConfigurationPropertyOptions.IsRequired)]
        public ServiceProviderEndpointCollection Endpoints
        {
            get { return (ServiceProviderEndpointCollection)base["endpoints"]; }
            set { base["endpoints"] = value; }
        }

        /// <summary>
        /// Gets or sets the name id formats.
        /// </summary>
        /// <value>The name id formats.</value>
        [ConfigurationProperty("nameIdFormats")]
        public NameIdFormatCollection NameIdFormats
        {
            get { return (NameIdFormatCollection)base["nameIdFormats"]; }
            set { base["nameIdFormats"] = value; }
        }

        /// <summary>
        /// Gets or sets the signing certificate.
        /// </summary>
        /// <value>The signing certificate.</value>
        [ConfigurationProperty("signingCertificate")]
        public CertificateElement SigningCertificate
        {
            get { return (CertificateElement)base["signingCertificate"]; }
            set { base["signingCertificate"] = value; }
        }

        #endregion
    }
}
