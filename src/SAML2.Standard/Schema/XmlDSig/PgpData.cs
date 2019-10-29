using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// The <c>PGPData</c> element within KeyInfo is used to convey information related to PGP public key pairs and 
    /// signatures on such keys. The PGPKeyID's value is a base64Binary sequence containing a standard PGP public 
    /// key identifier as defined in [PGP, section 11.2]. The <c>PgpKeyPacket</c> contains a base64-encoded Key Material 
    /// Packet as defined in [PGP, section 5.5]. These children element types can be complemented/extended by 
    /// siblings from an external namespace within PGPData, or PGPData can be replaced all together with an 
    /// alternative PGP XML structure as a child of KeyInfo. PGPData must contain one <c>PGPKeyID</c> and/or one 
    /// <c>PgpKeyPacket</c> and 0 or more elements from an external namespace. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class PgpData
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "PGPData";

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Items are of type <c>PGPKeyID</c> or <c>PgpKeyPacket</c>
        /// </summary>
        /// <value>The items.</value>
        [XmlAnyElement]
        [XmlElement("PGPKeyID", typeof(byte[]), DataType = "base64Binary")]
        [XmlElement("PGPKeyPacket", typeof(byte[]), DataType = "base64Binary")]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items { get; set; }

        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public PgpItemType[] ItemsElementName { get; set; }

        #endregion
    }
}
