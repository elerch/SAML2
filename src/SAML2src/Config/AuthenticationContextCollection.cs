using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration collection.
    /// </summary>
    [ConfigurationCollection(typeof(AuthenticationContextElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class AuthenticationContextCollection : EnumerableConfigurationElementCollection<AuthenticationContextElement>
    {
        /// <summary>
        /// Gets the comparison.
        /// </summary>
        [ConfigurationProperty("comparison", DefaultValue = AuthenticationContextComparison.Exact)]
        public AuthenticationContextComparison Comparison
        {
            get { return (AuthenticationContextComparison)base["comparison"]; }
        }
    }
}
