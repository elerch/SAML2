using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The <c>SpSsoDescriptor</c> element extends SSODescriptorType with content reflecting profiles specific
    /// to service providers.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class SpSsoDescriptor : SsoDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "SPSSODescriptor";

        /// <summary>
        /// <c>AuthnRequestsSigned</c>c> backing field.
        /// </summary>
        private bool? _authnRequestsSignedField;

        /// <summary>
        /// <c>WantAssertionsSigned</c> backing field.
        /// </summary>
        private bool? _wantAssertionsSignedField;

        #region Attributes
        
        /// <summary>
        /// Gets or sets a value indicating whether the authentication requests is signed.
        /// Optional attribute that indicates whether the <c>samlp:AuthnRequest</c> messages sent by this
        /// service provider will be signed. If omitted, the value is assumed to be false.
        /// </summary>
        /// <value><c>true</c> if authentication requests signed; otherwise, <c>false</c>.</value>
        [XmlAttribute]
        public string AuthnRequestsSigned
        {
            get { return _authnRequestsSignedField == null ? null : XmlConvert.ToString(_authnRequestsSignedField.Value); }
            set { _authnRequestsSignedField = string.IsNullOrEmpty(value) ? (bool?)null : XmlConvert.ToBoolean(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether assertions should be signed.
        /// Optional attribute that indicates a requirement for the <c>saml:Assertion</c> elements received by
        /// this service provider to be signed. If omitted, the value is assumed to be false. This requirement
        /// is in addition to any requirement for signing derived from the use of a particular profile/binding
        /// combination.
        /// </summary>
        /// <value>
        /// <c>true</c> if want assertions signed; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public string WantAssertionsSigned
        {
            get { return _wantAssertionsSignedField == null ? null : XmlConvert.ToString(_wantAssertionsSignedField.Value); }
            set { _wantAssertionsSignedField = string.IsNullOrEmpty(value) ? (bool?)null : XmlConvert.ToBoolean(value); }
        }

        #endregion

        #region Elements
        
        /// <summary>
        /// Gets or sets the assertion consumer service.
        /// One or more elements that describe indexed endpoints that support the profiles of the
        /// Authentication Request protocol defined in [SAMLProf]. All service providers support at least one
        /// such endpoint, by definition.
        /// </summary>
        /// <value>The assertion consumer service.</value>
        [XmlElement("AssertionConsumerService", Order = 1)]
        public IndexedEndpoint[] AssertionConsumerService { get; set; }

        /// <summary>
        /// Gets or sets the attribute consuming service.
        /// Zero or more elements that describe an application or service provided by the service provider
        /// that requires or desires the use of SAML attributes.
        /// </summary>
        /// <value>The attribute consuming service.</value>
        [XmlElement("AttributeConsumingService", Order = 2)]
        public AttributeConsumingService[] AttributeConsumingService { get; set; }

        #endregion
    }
}
