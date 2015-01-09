using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Schema.XmlDSig;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;EntityDescriptor&gt; element specifies metadata for a single SAML entity. A single entity may act
    /// in many different roles in the support of multiple profiles. This specification directly supports the following
    /// concrete roles as well as the abstract &lt;RoleDescriptor&gt; element for extensibility (see subsequent
    /// sections for more details):
    /// * SSO Identity Provider
    /// * SSO Service Provider
    /// * Authentication Authority
    /// * Attribute Authority
    /// * Policy Decision Point
    /// * Affiliation
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class EntityDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EntityDescriptor";

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
        /// Gets or sets the entity ID.
        /// Specifies the unique identifier of the SAML entity whose metadata is described by the element's
        /// contents.
        /// </summary>
        /// <value>The entity ID.</value>
        [XmlAttribute("entityID", DataType = "anyURI")]
        public string EntityID { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// A document-unique identifier for the element, typically used as a reference point when signing
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("ID", DataType = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the valid until string.
        /// Optional attribute indicates the expiration time of the metadata contained in the element and any
        /// contained elements.
        /// </summary>
        /// <value>The valid until string.</value>
        [XmlAttribute("validUntil")]
        public string ValidUntilString
        {
            get { return ValidUntil == null ? null : ValidUntil.Value.ToUniversalTime().ToString("o"); }
            set { ValidUntil = value == null ? (DateTime?)null : DateTime.Parse(value); }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the additional metadata location.
        /// Optional sequence of namespace-qualified locations where additional metadata exists for the
        /// SAML entity. This may include metadata in alternate formats or describing adherence to other
        /// non-SAML specifications.
        /// </summary>
        /// <value>The additional metadata location.</value>
        [XmlElement("AdditionalMetadataLocation", Order = 6)]
        public AdditionalMetadataLocation[] AdditionalMetadataLocation { get; set; }

        /// <summary>
        /// Gets or sets the contact person.
        /// Optional sequence of elements identifying various kinds of contact personnel.
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
        /// Gets or sets the items.
        /// <c>&lt;RoleDescriptor&gt;</c>, <c>&lt;IdpSsoDescriptor&gt;</c>, <c>&lt;SpSsoDescriptor&gt;,</c>
        /// <c>&lt;AuthnAuthorityDescriptor&gt;</c>, <c>&lt;AttributeAuthorityDescriptor&gt;</c>, <c>&lt;PDPDescriptor&gt;</c>
        /// <c>&lt;AffiliationDescriptor&gt;</c>
        /// </summary>
        /// <value>The items.</value>
        [XmlElement(AffiliationDescriptor.ElementName, typeof(AffiliationDescriptor), Order = 3)]
        [XmlElement(AttributeAuthorityDescriptor.ElementName, typeof(AttributeAuthorityDescriptor), Order = 3)]
        [XmlElement(AuthnAuthorityDescriptor.ElementName, typeof(AuthnAuthorityDescriptor), Order = 3)]
        [XmlElement(IdpSsoDescriptor.ElementName, typeof(IdpSsoDescriptor), Order = 3)]
        [XmlElement(PdpDescriptor.ElementName, typeof(PdpDescriptor), Order = 3)]
        [XmlElement(RoleDescriptor.ElementName, typeof(RoleDescriptor), Order = 3)]
        [XmlElement(SpSsoDescriptor.ElementName, typeof(SpSsoDescriptor), Order = 3)]
        public object[] Items { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// Optional element identifying the organization responsible for the SAML entity described by the
        /// element.
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
