using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The complex type IndexedEndpointType extends EndpointType with a pair of attributes to permit the
    /// indexing of otherwise identical endpoints so that they can be referenced by protocol messages.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class IndexedEndpoint : Endpoint
    {   
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "ArtifactResolutionService";

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// An optional boolean attribute used to designate the default endpoint among an indexed set. If
        /// omitted, the value is assumed to be false.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool? IsDefault { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the index.
        /// A required attribute that assigns a unique integer value to the endpoint so that it can be
        /// referenced in a protocol message. The index value need only be unique within a collection of like
        /// elements contained within the same parent element (i.e., they need not be unique across the
        /// entire instance).
        /// </summary>
        /// <value>The index.</value>
        [XmlAttribute("index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the isDefault string.
        /// </summary>
        /// <value>The isDefault string.</value>
        [XmlAttribute("isDefault")]
        public string IsDefaultString
        {
            get { return IsDefault == null ? null : XmlConvert.ToString(IsDefault.Value); }
            set { IsDefault = value == null ? (bool?)null : XmlConvert.ToBoolean(value); }
        }

        #endregion
    }
}
