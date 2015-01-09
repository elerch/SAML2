using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration collection.
    /// </summary>
    [ConfigurationCollection(typeof(NameIdFormatElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class NameIdFormatCollection : EnumerableConfigurationElementCollection<NameIdFormatElement>
    {
        /// <summary>
        /// Gets a value indicating whether to allow creation of new NameIdFormats.
        /// </summary>
        [ConfigurationProperty("allowCreate")]
        public bool AllowCreate
        {
            get { return (bool)base["allowCreate"]; }
        }
    }
}
