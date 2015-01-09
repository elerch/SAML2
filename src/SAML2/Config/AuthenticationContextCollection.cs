using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration collection.
    /// </summary>
    public class AuthenticationContextCollection : EnumerableConfigurationElementCollection<AuthenticationContextElement>
    {
        /// <summary>
        /// Gets the comparison.
        /// </summary>
        public AuthenticationContextComparison Comparison
        {
            get { return (AuthenticationContextComparison)base["comparison"]; }
        }
    }
}
