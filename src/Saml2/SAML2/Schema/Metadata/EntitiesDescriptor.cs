using System;
using System.Xml.Serialization;
using SAML2.Schema.XmlDSig;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;EntitiesDescriptor&gt; element contains the metadata for an optionally named group of SAML
    /// entities.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class EntitiesDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EntitiesDescriptor";

        /// <summary>
        /// Gets or sets the valid until.
        /// Optional attribute indicates the expiration time of the metadata contained in the element and any
        /// contained elements.
        /// </summary>
        /// <value>The valid until.</value>
        [XmlIgnore]
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [valid until specified].
        /// </summary>
        /// <value><c>true</c> if [valid until specified]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool ValidUntilSpecified { get; set; }

        #region Attributes

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
        /// A document-unique identifier for the element, typically used as a reference point when signing
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("ID", DataType = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// A string name that identifies a group of SAML entities in the context of some deployment.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the valid until string.
        /// </summary>
        /// <value>The valid until string.</value>
        [XmlAttribute("validUntil")]
        public string ValidUntilString
        {
            get { return ValidUntil.ToUniversalTime().ToString("o"); }
            set { ValidUntil = DateTime.Parse(value); }
        }

        #endregion

        #region Elements

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
        /// Contains the metadata for one or more SAML entities, or a nested group of additional metadata
        /// </summary>
        /// <value>The items.</value>
        [XmlElement(ElementName, typeof(EntitiesDescriptor), Order = 3)]
        [XmlElement(EntityDescriptor.ElementName, typeof(EntityDescriptor), Order = 3)]
        public object[] Items { get; set; }

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
