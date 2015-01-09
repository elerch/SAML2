using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using SAML2.Schema.Core;
using SAML2.Schema.XmlDSig;
using SAML2.Utils;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// All SAML responses are of types that are derived from the StatusResponseType complex type. This type
    /// defines common attributes and elements that are associated with all SAML responses
    /// </summary>
    [XmlInclude(typeof(NameIdMappingResponse))]
    [XmlInclude(typeof(ArtifactResponse))]
    [XmlInclude(typeof(LogoutResponse))]
    [XmlInclude(typeof(Response))]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class StatusResponse
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "ManageNameIDResponse";

        /// <summary>
        /// Gets or sets the issue instant.
        /// The time instant of issue of the response.
        /// </summary>
        /// <value>The issue instant.</value>
        [XmlIgnore]
        public DateTime? IssueInstant { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the consent.
        /// Indicates whether or not (and under what conditions) consent has been obtained from a principal in
        /// the sending of this response. See Section 8.4 for some URI references that MAY be used as the value
        /// of the Consent attribute and their associated descriptions. If no Consent value is provided, the
        /// identifier <c>urn:oasis:names:tc:SAML:2.0:consent:unspecified</c> (see Section 8.4.1) is in
        /// effect.
        /// </summary>
        /// <value>The consent.</value>
        [XmlAttribute("Consent", DataType = "anyURI")]
        public string Consent { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// A URI reference indicating the address to which this response has been sent. This is useful to prevent
        /// malicious forwarding of responses to unintended recipients, a protection that is required by some
        /// protocol bindings. If it is present, the actual recipient MUST check that the URI reference identifies the
        /// location at which the message was received. If it does not, the response MUST be discarded. Some
        /// protocol bindings may require the use of this attribute (see [SAMLBind]).
        /// </summary>
        /// <value>The destination.</value>
        [XmlAttribute("Destination", DataType = "anyURI")]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// An identifier for the response. It is of type <c>xs:ID</c>, and MUST follow the requirements specified in
        /// Section 1.3.4 for identifier uniqueness.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("ID", DataType = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the in response to.
        /// A reference to the identifier of the request to which the response corresponds, if any. If the response
        /// is not generated in response to a request, or if the ID attribute value of a request cannot be
        /// determined (for example, the request is malformed), then this attribute MUST NOT be present.
        /// Otherwise, it MUST be present and its value MUST match the value of the corresponding request's
        /// ID attribute.
        /// </summary>
        /// <value>The in response to.</value>
        [XmlAttribute("InResponseTo", DataType = "NCName")]
        public string InResponseTo { get; set; }

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
        /// The version of this response. The identifier for the version of SAML defined in this specification is "2.0".
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute]
        public string Version { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the extensions.
        /// This extension point contains optional protocol message extension elements that are agreed on
        /// between the communicating parties. . No extension schema is required in order to make use of this
        /// extension point, and even if one is provided, the lax validation setting does not impose a requirement
        /// for the extension to be valid. SAML extension elements MUST be namespace-qualified in a non-
        /// SAML-defined namespace.
        /// </summary>
        /// <value>The extensions.</value>
        [XmlElement("Extensions", Order = 3)]
        public Extensions Extensions { get; set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// Identifies the entity that generated the response message.
        /// </summary>
        /// <value>The issuer.</value>
        [XmlElement("Issuer", Namespace = Saml20Constants.Assertion, Form = XmlSchemaForm.Qualified, Order = 1)]
        public NameId Issuer { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// An XML Signature that authenticates the responder and provides message integrity
        /// </summary>
        /// <value>The signature.</value>
        [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 2)]
        public Signature Signature { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// A code representing the status of the corresponding request
        /// </summary>
        /// <value>The status.</value>
        [XmlElement("Status", Order = 4)]
        public Status Status { get; set; }

        #endregion
    }
}
