using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Http Basic Authentication configuration element.
    /// </summary>
    public class HttpAuthElement : WritableConfigurationElement
    {
        #region Elements

        /// <summary>
        /// Gets or sets the clientCertificate in web.config to enable client certificate authentication.
        /// </summary>
        [ConfigurationProperty("clientCertificate")]
        public CertificateElement ClientCertificate
        {
            get { return (CertificateElement)base["clientCertificate"]; }
            set { base["clientCertificate"] = value; }
        }

        /// <summary>
        /// Gets or sets the credentials to use for artifact resolution.
        /// </summary>
        [ConfigurationProperty("credentials")]
        public HttpAuthCredentialsElement Credentials
        {
            get { return (HttpAuthCredentialsElement)base["credentials"]; }
            set { base["credentials"] = value; }
        }

        #endregion
    }
}
