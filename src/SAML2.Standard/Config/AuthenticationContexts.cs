using System.Collections.Generic;
using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration collection.
    /// </summary>
    public class AuthenticationContexts : List<AuthenticationContext>
    {
        public AuthenticationContexts() : base() { }
        public AuthenticationContexts(IEnumerable<AuthenticationContext> collection) : base(collection) { }
        /// <summary>
        /// Gets the comparison.
        /// </summary>
        public AuthenticationContextComparison Comparison { get; set; }
    }
}
