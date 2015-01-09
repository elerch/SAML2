using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Assertion Profile configuration element.
    /// </summary>
    public class AssertionProfileElement : WritableConfigurationElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the assertion validator.
        /// </summary>
        /// <value>The assertion validator.</value>
        [ConfigurationProperty("assertionValidator", IsRequired = true)]
        public string AssertionValidator
        {
            get { return (string)base["assertionValidator"]; }
            set { base["assertionValidator"] = value; }
        }

        #endregion
    }
}
