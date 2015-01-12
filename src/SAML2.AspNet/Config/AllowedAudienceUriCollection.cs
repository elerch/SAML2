using System.Configuration;

namespace SAML2.AspNet.Config
{
    /// <summary>
    /// Allows Audience configuration element.
    /// </summary>
    [ConfigurationCollection(typeof(AudienceUriElement), AddItemName = "audience", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class AllowedAudienceUriCollection : EnumerableConfigurationElementCollection<AudienceUriElement>
    {
    }
}
