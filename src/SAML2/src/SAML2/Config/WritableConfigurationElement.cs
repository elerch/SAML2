using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Provides a configuration element that is writable by default.
    /// </summary>
    public class WritableConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElementCollection"/> object is read only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Configuration.ConfigurationElementCollection"/> object is read only; otherwise, false.</returns>
        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
