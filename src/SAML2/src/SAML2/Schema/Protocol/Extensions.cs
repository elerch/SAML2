using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// This extension point contains optional protocol message extension elements that are agreed on
    /// between the communicating parties. No extension schema is required in order to make use of this
    /// extension point, and even if one is provided, the lax validation setting does not impose a requirement
    /// for the extension to be valid. SAML extension elements MUST be namespace-qualified in a non-
    /// SAML-defined namespace.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class Extensions
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Extensions";

        #region Elements

        /// <summary>
        /// Gets or sets the any XML element.
        /// </summary>
        /// <value>The Any XML element.</value>
        [XmlAnyElement(Order = 1)]
        public XmlElement[] Any { get; set; }

        #endregion
    }
}
