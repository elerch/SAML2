using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Schema.XmlDSig;
using SAML2.Utils;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;RoleDescriptor&gt; element is an abstract extension point that contains common descriptive
    /// information intended to provide processing commonality across different roles. New roles can be defined
    /// by extending its abstract RoleDescriptorType complex type
    /// </summary>
    [XmlInclude(typeof(AttributeAuthorityDescriptor))]
    [XmlInclude(typeof(PdpDescriptor))]
    [XmlInclude(typeof(AuthnAuthorityDescriptor))]
    [XmlInclude(typeof(SsoDescriptor))]
    [XmlInclude(typeof(SpSsoDescriptor))]
    [XmlInclude(typeof(IdpSsoDescriptor))]    
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public abstract class RoleDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "RoleDescriptor";

        /// <summary>
        /// The protocol support enumeration backing field
        /// </summary>
        private string _protocolSupportEnumeration;

        /// <summary>
        /// Gets or sets the protocol support enumeration.
        /// A whitespace-delimited set of URIs that identify the set of protocol specifications supported by the
        /// role element. For SAML V2.0 entities, this set MUST include the SAML protocol namespace URI,
        /// <c>urn:oasis:names:tc:SAML:2.0:protocol</c>. Note that future SAML specifications might
        /// share the same namespace URI, but SHOULD provide alternate "protocol support" identifiers to
        /// ensure discrimination when necessary.
        /// </summary>
        /// <value>The protocol support enumeration.</value>
        [XmlIgnore]
        public string[] ProtocolSupportEnumeration
        {
            get
            {
                return _protocolSupportEnumeration.Split(' ');
            }

            set
            {
                _protocolSupportEnumeration = string.Join(" ", value);
            } 
        }

        /// <summary>
        /// Gets or sets the valid until.
        /// Optional attribute indicates the expiration time of the metadata contained in the element and any
        /// contained elements.
        /// </summary>
        /// <value>The valid until.</value>
        [XmlIgnore]
        public DateTime? ValidUntil { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets XML any attribute.
        /// </summary>
        /// <value>The XML Any attribute.</value>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }
        
        /// <summary>
        /// Gets or sets the cache duration.
        /// Optional attribute indicates the maximum length of time a consumer should cache the metadata
        /// contained in the element and any contained elements.
        /// </summary>
        /// <value>The cache duration.</value>
        [XmlAttribute("cacheDuration", DataType = "duration")]
        public string CacheDuration { get; set; }

        /// <summary>
        /// Gets or sets the error URL.
        /// Optional URI attribute that specifies a location to direct a user for problem resolution and
        /// additional support related to this role.
        /// </summary>
        /// <value>The error URL.</value>
        [XmlAttribute("errorUrl", DataType = "anyURI")]
        public string ErrorUrl { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// A document-unique identifier for the element, typically used as a reference point when signing.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("ID", DataType = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the protocol support enumeration string.
        /// </summary>
        /// <value>The protocol support enumeration string.</value>
        [XmlAttribute("protocolSupportEnumeration", DataType = "anyURI")]
        public string ProtocolSupportEnumerationString
        {
            get { return _protocolSupportEnumeration; }
            set { _protocolSupportEnumeration = value;  }
        }

        /// <summary>
        /// Gets or sets the valid until string.
        /// </summary>
        /// <value>The valid until string.</value>
        [XmlAttribute("validUntil")]
        public string ValidUntilString
        {
            get { return ValidUntil.HasValue ? Saml20Utils.ToUtcString(ValidUntil.Value) : null; }
            set { ValidUntil = string.IsNullOrEmpty(value) ? (DateTime?)null : Saml20Utils.FromUtcString(value); }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the contact person.
        /// Optional sequence of elements specifying contacts associated with this role. Identical to the
        /// element used within the &lt;EntityDescriptor&gt; element.
        /// </summary>
        /// <value>The contact person.</value>
        [XmlElement("ContactPerson", Order = 5)]
        public Contact[] ContactPerson { get; set; }

        /// <summary>
        /// Gets or sets the extensions.
        /// This contains optional metadata extensions that are agreed upon between a metadata publisher
        /// and consumer. Extension elements MUST be namespace-qualified by a non-SAML-defined
        /// namespace.
        /// </summary>
        /// <value>The extensions.</value>
        [XmlElement("Extensions", Order = 2)]
        public ExtensionType Extensions { get; set; }

        /// <summary>
        /// Gets or sets the key descriptor.
        /// Optional sequence of elements that provides information about the cryptographic keys that the
        /// entity uses when acting in this role.
        /// </summary>
        /// <value>The key descriptor.</value>
        [XmlElement("KeyDescriptor", Order = 3)]
        public KeyDescriptor[] KeyDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// Optional element specifies the organization associated with this role. Identical to the element used
        /// within the &lt;EntityDescriptor&gt; element.
        /// </summary>
        /// <value>The organization.</value>
        [XmlElement("Organization", Order = 4)]
        public Organization Organization { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// An XML signature that authenticates the containing element and its contents
        /// </summary>
        /// <value>The signature.</value>
        [XmlElement("Signature", Namespace = Saml20Constants.Xmldsig, Order = 1)]
        public Signature Signature { get; set; }

        #endregion
    }
}
