using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// The CipherData is a mandatory element that provides the encrypted data. It must either contain the 
    /// encrypted octet sequence as base64 encoded text of the CipherValue element, or provide a reference to an 
    /// external location containing the encrypted octet sequence via the CipherReference element.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class CipherData
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "CipherData";

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        [XmlElement("CipherReference", typeof(CipherReference))]
        [XmlElement("CipherValue", typeof(byte[]), DataType = "base64Binary")]
        public object Item { get; set; }

        #endregion
    }
}
