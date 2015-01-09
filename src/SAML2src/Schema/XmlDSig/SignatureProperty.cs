using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// For the inclusion of assertions about the signature itself (e.g., signature semantics, the time of signing 
    /// or the serial number of hardware used in cryptographic processes). Such assertions may be signed by including 
    /// a Reference for the SignatureProperties in SignedInfo. While the signing application should be very careful 
    /// about what it signs (it should understand what is in the SignatureProperty) a receiving application has no 
    /// obligation to understand that semantic (though its parent trust engine may wish to). Any content about the 
    /// signature generation may be located within the SignatureProperty element. The mandatory Target attribute 
    /// references the Signature element to which the property applies. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class SignatureProperty
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SignatureProperty";

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlText]
        public string[] Text { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("Id", DataType = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        [XmlAttribute("Target", DataType = "anyURI")]
        public string Target { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [XmlAnyElement]
        public XmlElement[] Items { get; set; }

        #endregion
    }
}
