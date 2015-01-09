using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The <c>&lt;IdpSsoDescriptor&gt;</c> element extends SSODescriptorType with content reflecting profiles
    /// specific to identity providers supporting SSO.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class IdpSsoDescriptor : SsoDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "IDPSSODescriptor";

        #region Attributes

        /// <summary>
        /// Gets or sets a value indicating whether want authentication requests signed.
        /// Optional attribute that indicates a requirement for the <c>&lt;samlp:AuthnRequest&gt;</c> messages
        /// received by this identity provider to be signed. If omitted, the value is assumed to be false.
        /// </summary>
        /// <value>
        /// <c>true</c> if want authentication requests signed; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool WantAuthnRequestsSigned { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the assertion ID request service.
        /// Zero or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Assertion Request protocol defined in [SAMLProf] or the special URI binding for assertion
        /// requests defined in [SAMLBind].
        /// </summary>
        /// <value>The assertion ID request service.</value>
        [XmlElement("AssertionIDRequestService", Order = 3)]
        public Endpoint[] AssertionIdRequestService { get; set; }

        /// <summary>
        /// Gets or sets the attribute.
        /// Zero or more elements that identify the SAML attributes supported by the identity provider.
        /// Specific values MAY optionally be included, indicating that only certain values permitted by the
        /// attribute's definition are supported. In this context, "support" for an attribute means that the identity
        /// provider has the capability to include it when delivering assertions during single sign-on.
        /// </summary>
        /// <value>The attribute.</value>
        [XmlElement("Attribute", Namespace = Saml20Constants.Assertion, Order = 5)]
        public SamlAttribute[] Attributes { get; set; }

        /// <summary>
        /// Gets or sets the attribute profile.
        /// Zero or more elements of type anyURI that enumerate the attribute profiles supported by this
        /// identity provider. See [SAMLProf] for some possible values for this element.
        /// </summary>
        /// <value>The attribute profile.</value>
        [XmlElement("AttributeProfile", DataType = "anyURI", Order = 4)]
        public string[] AttributeProfile { get; set; }

        /// <summary>
        /// Gets or sets the name ID mapping service.
        /// Zero or more elements of type EndpointType that describe endpoints that support the Name
        /// Identifier Mapping profile defined in [SAMLProf]. The ResponseLocation attribute MUST be
        /// omitted.
        /// </summary>
        /// <value>The name ID mapping service.</value>
        [XmlElement("NameIDMappingService", Order = 2)]
        public Endpoint[] NameIdMappingService { get; set; }

        /// <summary>
        /// Gets or sets the single sign on service.
        /// One or more elements of type EndpointType that describe endpoints that support the profiles of
        /// the Authentication Request protocol defined in [SAMLProf]. All identity providers support at least
        /// one such endpoint, by definition. The ResponseLocation attribute MUST be omitted.
        /// </summary>
        /// <value>The single sign on service.</value>
        [XmlElement("SingleSignOnService", Order = 1)]
        public Endpoint[] SingleSignOnService { get; set; }

        #endregion
    }
}
