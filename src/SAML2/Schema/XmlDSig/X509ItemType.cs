using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// X509 item types.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig, IncludeInSchema = false)]
    public enum X509ItemType
    {
        /// <summary>
        /// Any item type.
        /// </summary>
        [XmlEnum("##any:")]
        Item,

        /// <summary>
        /// X509CRL item type.
        /// </summary>
        [XmlEnum("X509CRL")]
        X509CRL,
        
        /// <summary>
        /// X509Certificate item type.
        /// </summary>
        [XmlEnum("X509Certificate")]
        X509Certificate,
        
        /// <summary>
        /// X509IssuerSerial item type.
        /// </summary>
        [XmlEnum("X509IssuerSerial")]
        X509IssuerSerial,

        /// <summary>
        /// X509SKI item type.
        /// </summary>
        [XmlEnum("X509SKI")]
        X509SKI,

        /// <summary>
        /// X509SubjectName item type.
        /// </summary>
        [XmlEnum("X509SubjectName")]
        X509SubjectName
    }
}