using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Authentication Context configuration element.
    /// </summary>
    public class AuthenticationContext
    {
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

    }
}
