using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Authentication Context configuration element.
    /// </summary>
    public class AuthenticationContextElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        [ConfigurationProperty("context", IsKey = true, IsRequired = true)]
        public string Context
        {
            get { return (string)base["context"]; }
            set { base["context"] = value; }
        }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        /// <value>The reference type.</value>
        [ConfigurationProperty("referenceType", IsRequired = true)]
        public string ReferenceType
        {
            get { return (string)base["referenceType"]; }
            set { base["referenceType"] = value; }
        }

        #endregion

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Context; }
        }

        #endregion
    }
}
