using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// Additional information items concerning the generation of the EncryptedData or EncryptedKey can be placed 
    /// in an EncryptionProperty element (e.g., date/time stamp or the serial number of cryptographic hardware used 
    /// during encryption). The Target attribute identifies the EncryptedType structure being described. anyAttribute 
    /// permits the inclusion of attributes from the XML namespace to be included (i.e., xml:space, xml:lang, and 
    /// xml:base).
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class EncryptionProperty
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EncryptionProperty";

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlText]
        public string[] Text { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets XML any attribute.
        /// </summary>
        /// <value>The XML Any attribute.</value>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

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
