using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// Holds list of EncryptionProperty elements
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class EncryptionProperties
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EncryptionProperties";

        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("Id", DataType = "ID")]
        public string Id { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the encryption property.
        /// </summary>
        /// <value>The encryption property.</value>
        [XmlElement("EncryptionProperty")]
        public EncryptionProperty[] EncryptionProperty { get; set; }

        #endregion
    }
}
