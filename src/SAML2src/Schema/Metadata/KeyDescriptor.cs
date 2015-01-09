using System;
using System.Xml.Serialization;
using SAML2.Schema.XEnc;
using SAML2.Schema.XmlDSig;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;KeyDescriptor&gt; element provides information about the cryptographic key(s) that an entity uses
    /// to sign data or receive encrypted keys, along with additional cryptographic details.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class KeyDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "KeyDescriptor";

        /// <summary>
        /// Gets or sets a value indicating whether [use specified].
        /// </summary>
        /// <value><c>true</c> if [use specified]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool UseSpecified { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the use.
        /// Optional attribute specifying the purpose of the key being described. Values are drawn from the
        /// KeyTypes enumeration, and consist of the values encryption and signing.
        /// </summary>
        /// <value>The use.</value>
        [XmlAttribute("use")]
        public KeyTypes Use { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the encryption method.
        /// Optional element specifying an algorithm and algorithm-specific settings supported by the entity.
        /// The exact content varies based on the algorithm supported. See [XMLEnc] for the definition of this
        /// element's xenc:EncryptionMethodType complex type.
        /// </summary>
        /// <value>The encryption method.</value>
        [XmlElement("EncryptionMethod", Order = 2)]
        public EncryptionMethod[] EncryptionMethod { get; set; }

        /// <summary>
        /// Gets or sets the XML Signature element KeyInfo. Can be implicitly converted to the .NET class System.Security.Cryptography.Xml.KeyInfo.
        /// </summary>
        [XmlElement("KeyInfo", Namespace = Saml20Constants.Xmldsig, Order = 1)]
        public KeyInfo KeyInfo { get; set; }

        #endregion
    }
}
