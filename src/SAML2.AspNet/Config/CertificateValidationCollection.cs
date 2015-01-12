using System.Configuration;

namespace SAML2.AspNet.Config
{
    /// <summary>
    /// Certificate Validation configuration collection.
    /// </summary>
    [ConfigurationCollection(typeof(CertificateValidationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class CertificateValidationCollection : EnumerableConfigurationElementCollection<CertificateValidationElement>
    {
    }
}
