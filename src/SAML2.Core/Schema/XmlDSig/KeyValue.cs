using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// The KeyValue element contains a single public key that may be useful in validating the signature. 
    /// Structured formats for defining DSA (REQUIRED) and RSA (RECOMMENDED) public keys are defined in Signature 
    /// Algorithms (section 6.4). The KeyValue element may include externally defined public keys values 
    /// represented as PCDATA or element types from an external namespace. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class KeyValue
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "KeyValue";

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlText]
        public string[] Text { get; set; }

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// Item is of type DSAKeyValue or RSAKeyValue
        /// </summary>
        /// <value>The item.</value>
        [XmlAnyElement]
        [XmlElement("DSAKeyValue", typeof(DsaKeyValue))]
        [XmlElement("RSAKeyValue", typeof(RsaKeyValue))]
        public object Item { get; set; }

        #endregion
    }
}
