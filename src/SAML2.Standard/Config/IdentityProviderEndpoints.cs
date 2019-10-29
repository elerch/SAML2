using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SAML2.Config
{
    /// <summary>
    /// Identity Provider Endpoint configuration collection.
    /// </summary>
    public class IdentityProviderEndpoints : List<IdentityProviderEndpoint>
    {
        public IdentityProviderEndpoints() : base() { }
        public IdentityProviderEndpoints(IEnumerable<IdentityProviderEndpoint> collection) : base(collection) { }
        /// <summary>
        /// Gets the log off endpoint.
        /// </summary>
        public IdentityProviderEndpoint DefaultLogoutEndpoint
        {
            get { return this.FirstOrDefault(x => x.Type == EndpointType.Logout); }
        }

        /// <summary>
        /// Gets the sign on endpoint.
        /// </summary>
        public IdentityProviderEndpoint DefaultSignOnEndpoint
        {
            get { return this.FirstOrDefault(x => x.Type == EndpointType.SignOn); }
        }

    }
}
