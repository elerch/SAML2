using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Assertion Profile configuration element.
    /// </summary>
    public class AssertionProfileElement
    {
        /// <summary>
        /// Gets or sets the assertion validator.
        /// </summary>
        /// <value>The assertion validator.</value>
        public string AssertionValidator { get; set; }
    }
}
