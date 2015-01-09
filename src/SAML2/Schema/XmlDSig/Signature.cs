using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// The Signature element is the root element of an XML Signature
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class Signature 
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Signature";

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
        /// Gets or sets the signed info.
        /// The structure of SignedInfo includes the canonicalization algorithm, a signature algorithm, and one or 
        /// more references. The SignedInfo element may contain an optional ID attribute that will allow it to be 
        /// referenced by other signatures and objects. 
        /// </summary>
        /// <value>The signed info.</value>
        [XmlElement("SignedInfo")]
        public SignedInfo SignedInfo { get; set; }

        /// <summary>
        /// Gets or sets the signature value.
        /// </summary>
        /// <value>The signature value.</value>
        [XmlElement("SignatureValue")]
        public SignatureValue SignatureValue { get; set; }

        /// <summary>
        /// Gets or sets the key info.
        /// </summary>
        /// <value>The key info.</value>
        [XmlElement("KeyInfo")]
        public KeyInfo KeyInfo { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        [XmlElement("Object")]
        public ObjectType[] Object { get; set; }

        #endregion
    }
}
