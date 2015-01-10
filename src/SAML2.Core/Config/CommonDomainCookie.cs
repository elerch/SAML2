using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Common Domain Cookie configuration element.
    /// </summary>
    public class CommonDomainCookie {
        /// <summary>
        /// Gets or sets a value indicating whether Common Domain Cookie is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the local reader endpoint.
        /// </summary>
        /// <value>The local reader endpoint.</value>
        public string LocalReaderEndpoint { get; set; }
    }
}
