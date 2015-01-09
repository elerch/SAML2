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
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        /// <value>The reference type.</value>
        public string ReferenceType { get; set; }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey { get; set; }
    }
}
