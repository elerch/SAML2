using SAML2.Standard;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SAML2.Specification
{
    /// <summary>
    /// Checks if a certificate is within its validity period
    /// Performs an online revocation check if the certificate contains a CRL url (oid: 2.5.29.31)
    /// </summary>
    public class DefaultCertificateSpecification : ICertificateSpecification
    {
        /// <summary>
        /// Determines whether the specified certificate is considered valid according to the RFC3280 specification.
        /// </summary>
        /// <param name="certificate">The certificate to validate.</param>
        /// <returns>
        /// <c>true</c> if valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSatisfiedBy(X509Certificate2 certificate)
        {
            try
            {
                return certificate.Verify();
            }
            catch (Exception e)
            {
                Logging.LoggerProvider.LoggerFor(GetType()).Warn(string.Format(ErrorMessages.CertificateIsNotRFC3280Valid, certificate.SubjectName.Name, certificate.Thumbprint), e);
            }

            return false;
        }
    }
}
