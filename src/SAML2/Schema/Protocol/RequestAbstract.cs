using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;
using SAML2.Schema.XmlDSig;
using SAML2.Utils;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// All SAML requests are of types that are derived from the abstract RequestAbstractType complex type.
    /// This type defines common attributes and elements that are associated with all SAML requests
    /// </summary>
    [XmlInclude(typeof(NameIdMappingRequest))]
    [XmlInclude(typeof(LogoutRequest))]
    [XmlInclude(typeof(ManageNameIdRequest))]
    [XmlInclude(typeof(ArtifactResolve))]
    [XmlInclude(typeof(AuthnRequest))]
    [XmlInclude(typeof(SubjectQueryAbstract))]
    [XmlInclude(typeof(AuthzDecisionQuery))]
    [XmlInclude(typeof(AttributeQuery))]
    [XmlInclude(typeof(AuthnQuery))]
    [XmlInclude(typeof(AssertionIdRequest))]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    public abstract class RequestAbstract
    {
        /// <summary>
        /// Gets or sets the issue instant.
        /// The time instant of issue of the request.
        /// </summary>
        /// <value>The issue instant.</value>
        [XmlIgnore]
        public DateTime? IssueInstant { get; set; }

        #region Attributes
        
        /// <summary>
        /// Gets or sets the consent.
        /// Indicates whether or not (and under what conditions) consent has been obtained from a principal in
        /// the sending of this request.
        /// </summary>
        /// <value>The consent.</value>
        [XmlAttribute("Consent", DataType = "anyURI")]
        public string Consent { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// A URI reference indicating the address to which this request has been sent. This is useful to prevent
        /// malicious forwarding of requests to unintended recipients, a protection that is required by some
        /// protocol bindings. If it is present, the actual recipient MUST check that the URI reference identifies the
        /// location at which the message was received. If it does not, the request MUST be discarded. Some
        /// protocol bindings may require the use of this attribute (see [SAMLBind]).
        /// </summary>
        /// <value>The destination.</value>
        [XmlAttribute("Destination", DataType = "anyURI")]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the request. It is of type <c>xs:ID</c> and MUST follow the requirements specified in Section
        /// 1.3.4 for identifier uniqueness. The values of the ID attribute in a request and the InResponseTo
        /// attribute in the corresponding response MUST match.
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("ID", DataType = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the issue instant string.
        /// </summary>
        /// <value>The issue instant string.</value>
        [XmlAttribute("IssueInstant")]
        public string IssueInstantString
        {
            get { return IssueInstant.HasValue ? Saml20Utils.ToUtcString(IssueInstant.Value) : null; }
            set { IssueInstant = string.IsNullOrEmpty(value) ? (DateTime?)null : Saml20Utils.FromUtcString(value); }
        }

        /// <summary>
        /// Gets or sets the version.
        /// The version of this request. The identifier for the version of SAML defined in this specification is "2.0".
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute("Version")]
        public string Version { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the extensions.
        /// This extension point contains optional protocol message extension elements that are agreed on
        /// between the communicating parties. No extension schema is required in order to make use of this
        /// extension point, and even if one is provided, the lax validation setting does not impose a requirement
        /// for the extension to be valid. SAML extension elements MUST be namespace-qualified in a non-
        /// SAML-defined namespace
        /// </summary>
        /// <value>The extensions.</value>
        [XmlElement("Extensions", Order = 3)]
        public Extensions Extensions { get; set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// Identifies the entity that generated the request message.
        /// </summary>
        /// <value>The issuer.</value>
        [XmlElement("Issuer", Namespace = Saml20Constants.Assertion, Order = 1)]
        public NameId Issuer { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// An XML Signature that authenticates the requester and provides message integrity
        /// </summary>
        /// <value>The signature.</value>
        [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 2)]
        public Signature Signature { get; set; }

        #endregion
    }
}
