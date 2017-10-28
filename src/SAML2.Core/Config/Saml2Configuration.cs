using System.Collections.Generic;
using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// SAML2 Configuration Section.
    /// </summary>
    public class Saml2Configuration
    {
        /// <summary>
        /// Gets the section name.
        /// </summary>
        public static string Name { get { return "saml2"; } }

        /// <summary>
        /// Gets or sets the allowed audience uris.
        /// </summary>
        /// <value>The allowed audience uris.</value>
        public List<System.Uri> AllowedAudienceUris { get; set; }

        /// <summary>
        /// Gets or sets the assertion profile.
        /// </summary>
        /// <value>The assertion profile configuration.</value>
        public string AssertionProfileValidator { get; set; }
        /// <summary>
        /// Gets or sets the common domain cookie configuration.
        /// </summary>
        /// <value>The common domain cookie configuration.</value>
        public CommonDomainCookie CommonDomainCookie { get; set; }

        /// <summary>
        /// Gets or sets the identity providers.
        /// </summary>
        /// <value>The identity providers.</value>
        public IdentityProviders IdentityProviders { get; set; }
        /// <summary>
        /// Gets or sets the logging configuration.
        /// </summary>
        /// <value>The logging configuration.</value>
        public string LoggingFactoryType { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        public ServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets a value weather the response SAML message from IdP should be decompressed after it's BAse64 endoded.
        /// Compression is used by some IdP providers for example PingFederate.
        /// This perform these decode steps: https://www.samltool.com/decode.php (see Base64 Decode + Inflate page)
        /// Default: false
        /// </summary>
        /// <value>The response encoding.</value>
        public bool InflateResponseMessage { get; set; }

        public Saml2Configuration()
        {
            IdentityProviders = new IdentityProviders();
            AllowedAudienceUris = new List<System.Uri>();
            Metadata = new Metadata();
        }

    }
}
