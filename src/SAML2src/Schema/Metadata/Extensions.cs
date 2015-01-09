using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// Extension type
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "ExtensionsType", Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class ExtensionType
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Extensions";

        #region Elements

        /// <summary>
        /// Gets or sets any elements.
        /// </summary>
        /// <value>Any elements.</value>
        [XmlAnyElement(Order = 1)]
        public XmlElement[] Any { get; set; }

        #endregion
    }
}