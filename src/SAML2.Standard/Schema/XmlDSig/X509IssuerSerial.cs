using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// contains an X.509 issuer distinguished name/serial number pair that SHOULD be compliant with RFC2253 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    public class X509IssuerSerial
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "X509IssuerSerial";

        #region Elements

        /// <summary>
        /// Gets or sets the name of the X509 issuer.
        /// </summary>
        /// <value>The name of the X509 issuer.</value>
        [XmlElement("X509IssuerName")]
        public string X509IssuerName { get; set; }

        /// <summary>
        /// Gets or sets the X509 serial number.
        /// </summary>
        /// <value>The X509 serial number.</value>
        [XmlElement("X509SerialNumber", DataType = "integer")]
        public string X509SerialNumber { get; set; }

        #endregion
    }
}
