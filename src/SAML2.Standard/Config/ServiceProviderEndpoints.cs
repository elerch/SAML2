using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration collection.
    /// </summary>
    public class ServiceProviderEndpoints : List<ServiceProviderEndpoint>
    {
        public ServiceProviderEndpoints() : base() { }
        public ServiceProviderEndpoints(IEnumerable<ServiceProviderEndpoint> collection) : base(collection) { }
        /// <summary>
        /// Gets the log off endpoint.
        /// </summary>
        public ServiceProviderEndpoint DefaultLogoutEndpoint
        {
            get { return this.FirstOrDefault(x => x.Type == EndpointType.Logout); }
        }

        /// <summary>
        /// Gets the sign on endpoint.
        /// </summary>
        public ServiceProviderEndpoint DefaultSignOnEndpoint
        {
            get { return this.FirstOrDefault(x => x.Type == EndpointType.SignOn); }
        }
    }
}
