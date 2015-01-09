using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Http Basic Authentication configuration element.
    /// </summary>
    public class HttpAuthCredentialsElement : WritableConfigurationElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [ConfigurationProperty("username")]
        public string Username
        {
            get { return (string)base["username"]; }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }

        #endregion
    }
}
