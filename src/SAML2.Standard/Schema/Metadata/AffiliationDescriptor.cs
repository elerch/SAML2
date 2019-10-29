using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Schema.XmlDSig;
using SAML2.Utils;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;AffiliationDescriptor&gt; element is an alternative to the sequence of role descriptors
    /// described in Section 2.4 that is used when an &lt;EntityDescriptor&gt; describes an affiliation of SAML
    /// entities (typically service providers) rather than a single entity. The &lt;AffiliationDescriptor&gt;
    /// element provides a summary of the individual entities that make up the affiliation along with general
    /// information about the affiliation itself.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class AffiliationDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "AffiliationDescriptor";

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
        /// Gets or sets the affiliation owner ID.
        /// Specifies the unique identifier of the entity responsible for the affiliation. The owner is NOT
        /// presumed to be a member of the affiliation; if it is a member, its identifier MUST also appear in an
        /// &lt;AffiliateMember&gt; element.
        /// </summary>
        /// <value>The affiliation owner ID.</value>
        [XmlAttribute("affiliationOwnerID", DataType = "anyURI")]
        public string AffiliationOwnerId { get; set; }

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
        /// Gets or sets the ID.
        /// A document-unique identifier for the element, typically used as a reference point when signing.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("ID", DataType = "ID")]
        public string ID { get; set; }

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
        /// Gets or sets the affiliate member.
        /// One or more elements enumerating the members of the affiliation by specifying each member's
        /// unique identifier.
        /// </summary>
        /// <value>The affiliate member.</value>
        [XmlElement("AffiliateMember", DataType = "anyURI", Order = 3)]
        public string[] AffiliateMember { get; set; }

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
        /// affiliation uses as a whole, as distinct from keys used by individual members of the affiliation,
        /// which are published in the metadata for those entities.
        /// </summary>
        /// <value>The key descriptor.</value>
        [XmlElement("KeyDescriptor", Order = 4)]
        public KeyDescriptor[] KeyDescriptor { get; set; }

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
