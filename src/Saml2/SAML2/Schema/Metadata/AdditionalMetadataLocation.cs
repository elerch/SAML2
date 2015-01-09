using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;AdditionalMetadataLocation&gt; element is a namespace-qualified URI that specifies where
    /// additional XML-based metadata may exist for a SAML entity. Its AdditionalMetadataLocationType
    /// complex type extends the anyURI type with a namespace attribute (also of type anyURI). This required
    /// attribute MUST contain the XML namespace of the root element of the instance document found at the
    /// specified location.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class AdditionalMetadataLocation
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "AdditionalMetadataLocation";

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText(DataType = "anyURI")]
        public string Value { get; set; }
        
        #region Attributes

        /// <summary>
        /// Gets or sets the @namespace.
        /// </summary>
        /// <value>The @namespace.</value>
        [XmlAttribute("namespace", DataType = "anyURI")]
        public string Namespace { get; set; }

        #endregion
    }
}
