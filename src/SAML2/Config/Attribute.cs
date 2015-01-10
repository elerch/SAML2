using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Attribute configuration element.
    /// </summary>
    public class AttributeElement : IConfigurationElementCollectionElement
    {

        /// <summary>
        /// Gets or sets a value indicating whether this attribute is required.
        /// </summary>
        /// <value><c>true</c> if this attribute is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey { get; set; }
    }
}
