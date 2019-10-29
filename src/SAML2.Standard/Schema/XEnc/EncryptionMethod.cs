using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// EncryptionMethod is an optional element that describes the encryption algorithm applied to the cipher data. 
    /// If the element is absent, the encryption algorithm must be known by the recipient or the decryption will fail.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class EncryptionMethod
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EncryptionMethod";

        #region Attributes

        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        /// <value>The algorithm.</value>
        [XmlAttribute("Algorithm", DataType = "anyURI")]
        public string Algorithm { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets any elements.
        /// </summary>
        /// <value>Any elements.</value>
        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any { get; set; }

        /// <summary>
        /// Gets or sets the size of the key.
        /// </summary>
        /// <value>The size of the key.</value>
        [XmlElement("KeySize", DataType = "integer")]
        public string KeySize { get; set; }

        /// <summary>
        /// Gets or sets the OAEP parameters.
        /// </summary>
        /// <value>The OAEP parameters.</value>
        [XmlElement("OAEPparams", DataType = "base64Binary")]
        public byte[] OAEPparams { get; set; }

        #endregion
    }
}
