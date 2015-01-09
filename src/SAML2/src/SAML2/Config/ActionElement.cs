using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Action configuration element.
    /// </summary>
    public class ActionElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        #region Attributes

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

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
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
