using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;SubjectLocality&gt; element specifies the DNS domain name and IP address for the system from
    /// which the assertion subject was authenticated. It has the following attributes:
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class SubjectLocality
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SubjectLocality";

        #region Attributes

        /// <summary>
        /// Gets or sets the address.
        /// The network address of the system from which the principal identified by the subject was
        /// authenticated. IPv4 addresses SHOULD be represented in dotted-decimal format (e.g., "1.2.3.4").
        /// IPv6 addresses SHOULD be represented as defined by Section 2.2 of IETF RFC 3513 [RFC 3513]
        /// (e.g., "FEDC:BA98:7654:3210:FEDC:BA98:7654:3210").
        /// </summary>
        /// <value>The address.</value>
        [XmlAttribute("Address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the name of the DNS.
        /// The DNS name of the system from which the principal identified by the subject was authenticated.
        /// </summary>
        /// <value>The name of the DNS.</value>
        [XmlAttribute("DNSName")]
        public string DNSName { get; set; }

        #endregion
    }
}
