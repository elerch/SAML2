using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace SAML2.Specification
{
    /// <summary>
    /// Validates a self-signed certificate
    /// </summary>
    public class SelfIssuedCertificateSpecification : ICertificateSpecification
    {
        /// <summary>
        /// Determines whether the specified certificate is considered valid by this specification.
        /// Always returns true. No online validation attempted.
        /// </summary>
        /// <param name="certificate">The certificate to validate.</param>
        /// <returns>
        /// <c>true</c>.
        /// </returns>
        public bool IsSatisfiedBy(X509Certificate2 certificate)
        {
            var chainPolicy = new X509ChainPolicy
                                  {
                                      RevocationMode = X509RevocationMode.NoCheck,
                                      VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority
                                  };
            var defaultCertificateValidator = X509CertificateValidator.CreateChainTrustValidator(false, chainPolicy);

            try
            {
                defaultCertificateValidator.Validate(certificate);
                return true;
            }
            catch (Exception e)
            {
                Logging.LoggerProvider.LoggerFor(GetType()).Warn(string.Format(ErrorMessages.CertificateIsNotRFC3280Valid, certificate.SubjectName.Name, certificate.Thumbprint), e);
            }

            return false;
        }
    }
}
