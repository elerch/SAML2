using System.Configuration;
using System.Linq;

namespace SAML2.Config
{
    /// <summary>
    /// Identity Provider Endpoint configuration collection.
    /// </summary>
    [ConfigurationCollection(typeof(IdentityProviderEndpointElement), AddItemName = "endpoint", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class IdentityProviderEndpointCollection : EnumerableConfigurationElementCollection<IdentityProviderEndpointElement>
    {
        /// <summary>
        /// Gets the log off endpoint.
        /// </summary>
        public IdentityProviderEndpointElement LogoutEndpoint
        {
            get { return this.FirstOrDefault(x => x.Type == EndpointType.Logout); }
        }

        /// <summary>
        /// Gets the sign on endpoint.
        /// </summary>
        public IdentityProviderEndpointElement SignOnEndpoint
        {
            get { return this.FirstOrDefault(x => x.Type == EndpointType.SignOn); }
        }
    }
}
