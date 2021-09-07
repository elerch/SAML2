using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    [XmlType(Namespace = Saml20Constants.WsFederationNamespace)]
    public class ApplicationServiceType: RoleDescriptor {}
}
