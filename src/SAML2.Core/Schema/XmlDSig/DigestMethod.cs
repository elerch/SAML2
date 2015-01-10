using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// DigestMethod is a required element that identifies the digest algorithm to be applied to the signed 
    /// object. This element uses the general structure here for algorithms specified in Algorithm Identifiers 
    /// and Implementation Requirements (section 6.1). 
    /// </para>
    /// <para>
    /// If the result of the URI dereference and application of Transforms is an XPath node-set (or sufficiently 
    /// functional replacement implemented by the application) then it must be converted as described in the 
    /// Reference Processing Model (section  4.3.3.2). If the result of URI dereference and application of 
    /// transforms is an octet stream, then no conversion occurs (comments might be present if the Canonical 
    /// XML with Comments was specified in the Transforms). The digest algorithm is applied to the data octets 
    /// of the resulting octet stream.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class DigestMethod
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "DigestMethod";

        #region Attributes

        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        /// <value>The algorithm.</value>
        [XmlAttribute("Algorithm", DataType = "anyURI")]
        public string Algorithm { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets any elements.
        /// </summary>
        /// <value>Any elements.</value>
        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any { get; set; }

        #endregion
    }
}
