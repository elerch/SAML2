using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// DataReference elements are used to refer to EncryptedData elements that were encrypted 
    /// using the key defined in the enclosing EncryptedKey element. Multiple DataReference elements 
    /// can occur if multiple EncryptedData elements exist that are encrypted by the same key.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "ReferenceType", Namespace = Saml20Constants.Xenc)]
    public class ReferenceType
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "ReferenceType";

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
        /// Gets or sets any elements.
        /// </summary>
        /// <value>Any elements.</value>
        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        #endregion
    }
}
