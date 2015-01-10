using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Requested Attributes configuration collection.
    /// </summary>
    [ConfigurationCollection(typeof(Attribute), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class RequestedAttributes : EnumerableConfigurationElementCollection<Attribute>
    {
    }
}
