using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// To request that an identity provider issue an assertion with an authentication statement, a presenter
    /// authenticates to that identity provider (or relies on an existing security context) and sends it an
    /// <c>&lt;AuthnRequest&gt;</c> message that describes the properties that the resulting assertion needs to have to
    /// satisfy its purpose. Among these properties may be information that relates to the content of the assertion
    /// and/or information that relates to how the resulting &lt;Response&gt; message should be delivered to the
    /// requester. The process of authentication of the presenter may take place before, during, or after the initial
    /// delivery of the <c>&lt;AuthnRequest&gt;</c> message.
    /// The requester might not be the same as the presenter of the request if, for example, the requester is a
    /// relying party that intends to use the resulting assertion to authenticate or authorize the requested subject
    /// so that the relying party can decide whether to provide a service.
    /// The <c>&lt;AuthnRequest&gt;</c> message SHOULD be signed or otherwise authenticated and integrity protected
    /// by the protocol binding used to deliver the message.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class AuthnRequest : RequestAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "AuthnRequest";

        /// <summary>
        /// Gets or sets a value indicating whether [assertion consumer service index specified].
        /// </summary>
        /// <value>
        /// <c>true</c> if [assertion consumer service index specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool AssertionConsumerServiceIndexSpecified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [attribute consuming service index specified].
        /// </summary>
        /// <value>
        /// <c>true</c> if [attribute consuming service index specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool AttributeConsumingServiceIndexSpecified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force authentication.
        /// A Boolean value. If "true", the identity provider MUST authenticate the presenter directly rather than
        /// rely on a previous security context. If a value is not provided, the default is "false". However, if both
        /// <c>ForceAuthn</c> and <c>IsPassive</c> are "true", the identity provider MUST NOT freshly authenticate the
        /// presenter unless the constraints of IsPassive can be met.
        /// </summary>
        /// <value><c>true</c> if force authentication; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool? ForceAuthn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is passive.
        /// A Boolean value. If "true", the identity provider and the user agent itself MUST NOT visibly take control
        /// of the user interface from the requester and interact with the presenter in a noticeable fashion. If a
        /// value is not provided, the default is "false".
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is passive; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool? IsPassive { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the index of the assertion consumer service.
        /// Indirectly identifies the location to which the &lt;Response&gt; message should be returned to the
        /// requester. It applies only to profiles in which the requester is different from the presenter, such as the
        /// Web Browser SSO profile in [SAMLProf]. The identity provider MUST have a trusted means to map
        /// the index value in the attribute to a location associated with the requester. [SAMLMeta] provides one
        /// possible mechanism. If omitted, then the identity provider MUST return the &lt;Response&gt; message to
        /// the default location associated with the requester for the profile of use. If the index specified is invalid,
        /// then the identity provider MAY return an error &lt;Response&gt; or it MAY use the default location. This
        /// attribute is mutually exclusive with the AssertionConsumerServiceURL and ProtocolBinding
        /// attributes.
        /// </summary>
        /// <value>The index of the assertion consumer service.</value>
        [XmlAttribute("AssertionConsumerServiceIndex")]
        public ushort AssertionConsumerServiceIndex { get; set; }

        /// <summary>
        /// Gets or sets the assertion consumer service URL.
        /// Specifies by value the location to which the <c>&lt;Response&gt;</c> message MUST be returned to the
        /// requester. The responder MUST ensure by some means that the value specified is in fact associated
        /// with the requester. [SAMLMeta] provides one possible mechanism; signing the enclosing
        /// <c>&lt;AuthnRequest&gt;</c> message is another. This attribute is mutually exclusive with the
        /// AssertionConsumerServiceIndex attribute and is typically accompanied by the
        /// ProtocolBinding attribute.
        /// </summary>
        /// <value>The assertion consumer service URL.</value>
        [XmlAttribute("AssertionConsumerServiceURL", DataType = "anyURI")]
        public string AssertionConsumerServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the index of the attribute consuming service.
        /// Indirectly identifies information associated with the requester describing the SAML attributes the
        /// requester desires or requires to be supplied by the identity provider in the <c>&lt;Response&gt;</c> message. The
        /// identity provider MUST have a trusted means to map the index value in the attribute to information
        /// associated with the requester. [SAMLMeta] provides one possible mechanism. The identity provider
        /// MAY use this information to populate one or more <c>&lt;saml:AttributeStatement&gt;</c> elements in the
        /// assertion(s) it returns.
        /// </summary>
        /// <value>The index of the attribute consuming service.</value>
        [XmlAttribute("AttributeConsumingServiceIndex")]
        public ushort AttributeConsumingServiceIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force authentication specified.
        /// </summary>
        /// <value><c>true</c> if force authentication specified; otherwise, <c>false</c>.</value>
        [XmlAttribute("ForceAuthn")]
        public string ForceAuthnString
        {
            get { return ForceAuthn.HasValue ? ForceAuthn.Value.ToString().ToLower() : null; }
            set
            {
                bool val;
                if (!bool.TryParse(value, out val))
                {
                    return;
                }

                ForceAuthn = val;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is passive specified.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is passive specified; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("IsPassive")]
        public string IsPassiveString
        {
            get { return IsPassive.HasValue ? IsPassive.Value.ToString().ToLower() : "false"; }
            set
            {
                bool val;
                if (!bool.TryParse(value, out val))
                {
                    return;
                }

                IsPassive = val;
            }
        }

        /// <summary>
        /// Gets or sets the protocol binding.
        /// A URI reference that identifies a SAML protocol binding to be used when returning the &lt;Response&gt;
        /// message. See [SAMLBind] for more information about protocol bindings and URI references defined
        /// for them. This attribute is mutually exclusive with the AssertionConsumerServiceIndex attribute
        /// and is typically accompanied by the AssertionConsumerServiceURL attribute.
        /// </summary>
        /// <value>The protocol binding.</value>
        [XmlAttribute("ProtocolBinding", DataType = "anyURI")]
        public string ProtocolBinding { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// Specifies the human-readable name of the requester for use by the presenter's user agent or the
        /// identity provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        [XmlAttribute("ProviderName")]
        public string ProviderName { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the conditions.
        /// Specifies the SAML conditions the requester expects to limit the validity and/or use of the resulting
        /// assertion(s). The responder MAY modify or supplement this set as it deems necessary. The
        /// information in this element is used as input to the process of constructing the assertion, rather than as
        /// conditions on the use of the request itself.
        /// </summary>
        /// <value>The conditions.</value>
        [XmlElement("Conditions", Namespace = Saml20Constants.Assertion, Order = 3)]
        public Conditions Conditions { get; set; }

        /// <summary>
        /// Gets or sets the name ID policy.
        /// Specifies constraints on the name identifier to be used to represent the requested subject. If omitted,
        /// then any type of identifier supported by the identity provider for the requested subject can be used,
        /// constrained by any relevant deployment-specific policies, with respect to privacy, for example
        /// </summary>
        /// <value>The name ID policy.</value>
        /// <remarks>
        /// This is here for a reason. Some SAML implementations require that NameIDPolicy is the first element following issuer.
        /// </remarks>
        [XmlElement("NameIDPolicy", Order = 2)]
        public NameIdPolicy NameIdPolicy { get; set; }

        /// <summary>
        /// Gets or sets the requested authentication context.
        /// Specifies the requirements, if any, that the requester places on the authentication context that applies
        /// to the responding provider's authentication of the presenter. See Section 3.3.2.2.1 for processing rules
        /// regarding this element.
        /// </summary>
        /// <value>The requested authentication context.</value>
        [XmlElement("RequestedAuthnContext", Order = 4)]
        public RequestedAuthnContext RequestedAuthnContext { get; set; }

        /// <summary>
        /// Gets or sets the scoping.
        /// Specifies a set of identity providers trusted by the requester to authenticate the presenter, as well as
        /// limitations and context related to proxy of the <c>&lt;AuthnRequest&gt;</c> message to subsequent identity
        /// providers by the responder
        /// </summary>
        /// <value>The scoping.</value>
        [XmlElement("Scoping", Order = 5)]
        public Scoping Scoping { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// Specifies the requested subject of the resulting assertion(s). This may include one or more
        /// <c>&lt;saml:SubjectConfirmation&gt;</c> elements to indicate how and/or by whom the resulting assertions
        /// can be confirmed.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement("Subject", Namespace = Saml20Constants.Assertion, Order = 1)]
        public Subject Subject { get; set; }

        #endregion
    }
}
