using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// The structure of SignedInfo includes the canonicalization algorithm, a signature algorithm, and one or 
    /// more references. The SignedInfo element may contain an optional ID attribute that will allow it to be 
    /// referenced by other signatures and objects.
    /// </para>
    /// <para>
    /// SignedInfo does not include explicit signature or digest properties (such as calculation time, 
    /// cryptographic device serial number, etc.). If an application needs to associate properties with the 
    /// signature or digest, it may include such information in a SignatureProperties element within an Object 
    /// element.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class SignedInfo
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SignedInfo";

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
        /// Gets or sets the canonicalization method.
        /// </summary>
        /// <value>The canonicalization method.</value>
        [XmlElement("CanonicalizationMethod")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        [XmlElement("Reference")]
        public Reference[] Reference { get; set; }

        /// <summary>
        /// Gets or sets the signature method.
        /// </summary>
        /// <value>The signature method.</value>
        [XmlElement("SignatureMethod")]
        public SignatureMethod SignatureMethod { get; set; }

        #endregion
    }
}
