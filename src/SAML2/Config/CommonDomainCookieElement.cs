using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Common Domain Cookie configuration element.
    /// </summary>
    public class CommonDomainCookieElement : WritableConfigurationElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets a value indicating whether Common Domain Cookie is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }

        /// <summary>
        /// Gets or sets the local reader endpoint.
        /// </summary>
        /// <value>The local reader endpoint.</value>
        [ConfigurationProperty("localReaderEndpoint")]
        public string LocalReaderEndpoint
        {
            get { return (string)base["localReaderEndpoint"]; }
            set { base["localReaderEndpoint"] = value; }
        }

        #endregion
    }
}
