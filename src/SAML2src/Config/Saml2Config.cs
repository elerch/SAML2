using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Provides helper methods for getting the configuration.
    /// </summary>
    public class Saml2Config
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private static Saml2Section _config;

        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <returns>The <see cref="Saml2Section"/> config.</returns>
        public static Saml2Section GetConfig()
        {
            if (_config == null)
            {
                _config = ConfigurationManager.GetSection(Saml2Section.Name) as Saml2Section;

                if (_config == null)
                {
                    throw new ConfigurationErrorsException(string.Format("Configuration section \"{0}\" not found", typeof(Saml2Section).Name));
                }

                _config.IdentityProviders.Refresh();
            }

            return _config;
        }

        /// <summary>
        /// Refreshes the configuration section, so that next time it is read it is retrieved from the configuration file.
        /// </summary>
        public static void Refresh()
        {
            _config = null;
            ConfigurationManager.RefreshSection(Saml2Section.Name);
            _config = ConfigurationManager.GetSection(Saml2Section.Name) as Saml2Section;

            if (_config == null)
            {
                throw new ConfigurationErrorsException(string.Format("Configuration section \"{0}\" not found", typeof(Saml2Section).Name));
            }

            _config.IdentityProviders.Refresh();
        }
    }
}
