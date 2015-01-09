using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Http Basic Authentication configuration element.
    /// </summary>
    public class HttpAuthElement 
    {
        /// <summary>
        /// Gets or sets the clientCertificate in web.config to enable client certificate authentication.
        /// </summary>
        public CertificateElement ClientCertificate { get; set; }        /// <summary>
        /// Gets or sets the credentials to use for artifact resolution.
        /// </summary>
        public HttpAuthCredentialsElement Credentials { get; set; }
    }
}
