using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Http Basic Authentication configuration element.
    /// </summary>
    public class HttpAuth 
    {
        /// <summary>
        /// Gets or sets the clientCertificate in web.config to enable client certificate authentication.
        /// </summary>
        public Certificate ClientCertificate { get; set; }        /// <summary>
        /// Gets or sets the credentials to use for artifact resolution.
        /// </summary>
        public HttpAuthCredentials Credentials { get; set; }
    }
}
