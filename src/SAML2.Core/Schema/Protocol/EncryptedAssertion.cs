using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The <c>&lt;EncryptedAssertion&gt;</c> element represents an assertion in encrypted fashion, as defined by the
    /// XML Encryption Syntax and Processing specification [XMLEnc].
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class EncryptedAssertion : EncryptedElement
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "EncryptedAssertion";
    }
}
