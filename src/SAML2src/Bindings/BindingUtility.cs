using System.Configuration;
using SAML2.Config;

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
        public static bool ValidateConfiguration()
        {
            var config = Saml2Config.GetConfig();
            if (config == null)
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigMissingSaml2Element);
            }

            if (config.ServiceProvider == null)
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigMissingServiceProviderElement);
            }

            if (string.IsNullOrEmpty(config.ServiceProvider.Id))
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigMissingServiceProviderIdAttribute);
            }

            if (config.ServiceProvider.SigningCertificate == null)
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigMissingSigningCertificateElement);
            }

            // This will throw if no certificate or multiple certificates are found
            var certificate = config.ServiceProvider.SigningCertificate.GetCertificate();
            if (!certificate.HasPrivateKey)
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigSigningCertificateMissingPrivateKey);
            }

            if (config.IdentityProviders == null)
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigMissingIdentityProvidersElement);
            }

            if (config.IdentityProviders.MetadataLocation == null)
            {
                throw new ConfigurationErrorsException(ErrorMessages.ConfigMissingMetadataLocation);
            }

            return true;
        }
    }
}