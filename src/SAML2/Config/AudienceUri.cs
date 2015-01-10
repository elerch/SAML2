using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Audience configuration element.
    /// </summary>
    public class AudienceUriElement : IConfigurationElementCollectionElement
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey { get; set; }
    }
}
