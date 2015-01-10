using System;
using System.Configuration;
using System.Runtime.Serialization;

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

        public static object ConfigurationManager { get; private set; }


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

    [Serializable]
    internal class ConfigurationErrorsException : Exception
    {
        public ConfigurationErrorsException()
        {
        }

        public ConfigurationErrorsException(string message) : base(message)
        {
        }

        public ConfigurationErrorsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConfigurationErrorsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
