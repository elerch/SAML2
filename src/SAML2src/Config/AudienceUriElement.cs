using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Audience configuration element.
    /// </summary>
    public class AudienceUriElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        [ConfigurationProperty("uri", IsKey = true, IsRequired = true)]
        public string Uri
        {
            get { return (string)base["uri"]; }
            set { base["uri"] = value; }
        }

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Uri; }
        }

        #endregion
    }
}
