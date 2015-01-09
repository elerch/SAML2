using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// The CipherData is a mandatory element that provides the encrypted data. It must either contain the 
    /// encrypted octet sequence as base64 encoded text of the CipherValue element, or provide a reference to 
    /// an external location containing the encrypted octet sequence via the CipherReference element.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class CipherReference
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "CipherReference";

        #region Attributes

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        [XmlAttribute("URI", DataType = "anyURI")]
        public string URI { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        [XmlElement("Transforms")]
        public TransformsType Item { get; set; }

        #endregion
    }
}
