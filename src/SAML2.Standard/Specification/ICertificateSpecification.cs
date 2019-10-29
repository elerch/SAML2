using System.Security.Cryptography.X509Certificates;

namespace SAML2.Specification
{
    /// <summary>
    /// Specification interface for certificate validation
    /// </summary>
    public interface ICertificateSpecification 
    {
        /// <summary>
        /// Determines whether the specified certificate is considered valid by this specification.
        /// </summary>
        /// <param name="certificate">The certificate to validate.</param>
        /// <returns>
        /// <c>true</c> if valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsSatisfiedBy(X509Certificate2 certificate);
    }
}
