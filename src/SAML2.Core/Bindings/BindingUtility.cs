using SAML2.Config;
using System;

namespace SAML2.Bindings
{
    /// <summary>
    /// Utility functions for use in binding implementations.
    /// </summary>
    public class BindingUtility
    {
        /// <summary>
        /// Validates the SAML20Federation configuration.
        /// </summary>
        /// <returns>True if validation passes, false otherwise</returns>
        public static bool ValidateConfiguration(Saml2Configuration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config", ErrorMessages.ConfigMissingSaml2Element);
            }

            if (config.ServiceProvider == null)
            {
                throw new ArgumentOutOfRangeException("config", ErrorMessages.ConfigMissingServiceProviderElement);
            }

            if (string.IsNullOrEmpty(config.ServiceProvider.Id))
            {
                throw new ArgumentOutOfRangeException("config", ErrorMessages.ConfigMissingServiceProviderIdAttribute);
            }

            if (config.ServiceProvider.SigningCertificate == null)
            {
                throw new ArgumentOutOfRangeException("config", ErrorMessages.ConfigMissingSigningCertificateElement);
            }

            // This will throw if no certificate or multiple certificates are found
            var certificate = config.ServiceProvider.SigningCertificate;
            if (!certificate.HasPrivateKey)
            {
                throw new ArgumentOutOfRangeException("config", ErrorMessages.ConfigSigningCertificateMissingPrivateKey);
            }

            if (config.IdentityProviders == null)
            {
                throw new ArgumentOutOfRangeException("config", ErrorMessages.ConfigMissingIdentityProvidersElement);
            }

            return true;
        }
    }
}