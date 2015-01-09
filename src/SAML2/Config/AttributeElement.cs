using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Attribute configuration element.
    /// </summary>
    public class AttributeElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets a value indicating whether this attribute is required.
        /// </summary>
        /// <value><c>true</c> if this attribute is required; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("isRequired")]
        public bool IsRequired
        {
            get { return (bool)base["isRequired"]; }
            set { base["isRequired"] = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        #endregion

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Name; }
        }

        #endregion
    }
}
