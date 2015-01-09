using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// SignatureMethod is a required element that specifies the algorithm used for signature generation and 
    /// validation. This algorithm identifies all cryptographic functions involved in the signature operation 
    /// (e.g. hashing, public key algorithms, MACs, padding, etc.). This element uses the general structure 
    /// here for algorithms described in section 6.1: Algorithm Identifiers and Implementation Requirements. 
    /// While there is a single identifier, that identifier may specify a format containing multiple distinct 
    /// signature values. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class SignatureMethod
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SignatureMethod";

        #region Attributes

        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        /// <value>The algorithm.</value>
        [XmlAttribute("v", DataType = "anyURI")]
        public string Algorithm { get; set; }
        
        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the any XML element.
        /// </summary>
        /// <value>The Any XML element.</value>
        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any { get; set; }
        
        /// <summary>
        /// Gets or sets the length of the HMAC output.
        /// </summary>
        /// <value>The length of the HMAC output.</value>
        [XmlElement("HMACOutputLength", DataType = "integer")]
        public string HMACOutputLength { get; set; }

        #endregion
    }
}
