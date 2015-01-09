using System;
using System.Xml.Serialization;
using SAML2.Schema.XmlDSig;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// The base class for EncryptedKey and EncryptedData
    /// </summary>
    [XmlInclude(typeof(EncryptedKey))]
    [XmlInclude(typeof(EncryptedData))]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    public abstract class Encrypted
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        [XmlAttribute("Encoding", DataType = "anyURI")]
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("Id", DataType = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        [XmlAttribute("MimeType")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlAttribute("Type", DataType = "anyURI")]
        public string Type { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the encryption method.
        /// the RSA public key algorithm.
        /// </summary>
        /// <value>The encryption method.</value>
        [XmlElement("EncryptionMethod")]
        public EncryptionMethod EncryptionMethod { get; set; }

        /// <summary>
        /// Gets or sets the key info.
        /// </summary>
        /// <value>The key info.</value>
        [XmlElement("KeyInfo", Namespace = Saml20Constants.Xmldsig)]
        public KeyInfo KeyInfo { get; set; }

        /// <summary>
        /// Gets or sets the cipher data.
        /// </summary>
        /// <value>The cipher data.</value>
        [XmlElement("CipherData")]
        public CipherData CipherData { get; set; }

        /// <summary>
        /// Gets or sets the encryption properties.
        /// </summary>
        /// <value>The encryption properties.</value>
        [XmlElement("EncryptionProperties")]
        public EncryptionProperties EncryptionProperties { get; set; }

        #endregion
    }
}
