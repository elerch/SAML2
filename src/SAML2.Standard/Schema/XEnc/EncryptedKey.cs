using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// The EncryptedKey element is used to transport encryption keys from the originator to a known recipient(s). 
    /// It may be used as a stand-alone XML document, be placed within an application document, or appear inside 
    /// an EncryptedData element as a child of a ds:KeyInfo element. The key value is always encrypted to the 
    /// recipient(s). When EncryptedKey is decrypted the resulting octets are made available to the EncryptionMethod 
    /// algorithm without any additional processing.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class EncryptedKey : Encrypted
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EncryptedKey";

        #region Attributes

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        /// <value>The recipient.</value>
        [XmlAttribute("Recipient")]
        public string Recipient { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the name of the carried key.
        /// </summary>
        /// <value>The name of the carried key.</value>
        [XmlElement("CarriedKeyName")]
        public string CarriedKeyName { get; set; }

        /// <summary>
        /// Gets or sets the reference list.
        /// </summary>
        /// <value>The reference list.</value>
        [XmlElement("ReferenceList")]
        public ReferenceList ReferenceList { get; set; }

        #endregion
    }
}
