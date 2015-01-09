using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// The SPKIData element within KeyInfo is used to convey information related to SPKI public key pairs, 
    /// certificates and other SPKI data. SPKISexp is the base64 encoding of a SPKI canonical S-expression. 
    /// SPKIData must have at least one SPKISexp; SPKISexp can be complemented/extended by siblings from an 
    /// external namespace within SPKIData, or SPKIData can be entirely replaced with an alternative SPKI XML 
    /// structure as a child of KeyInfo. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class SpkiData
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SPKIData";

        #region Attributes

        /// <summary>
        /// Gets or sets any attributes.
        /// </summary>
        /// <value>Any attributes.</value>
        [XmlAnyElement]
        public XmlElement Any { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the SPKIS expression.
        /// </summary>
        /// <value>The SPKIS expression.</value>
        [XmlElement("SPKISexp", DataType = "base64Binary")]
        public byte[][] SPKISexp { get; set; }

        #endregion
    }
}
