using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Http Basic Authentication configuration element.
    /// </summary>
    public class HttpAuthCredentials
    {

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

    }
}
