using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Schema.Metadata;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;Attribute&gt; element identifies an attribute by name and optionally includes its value(s). It has the
    /// AttributeType complex type. It is used within an attribute statement to express particular attributes and
    /// values associated with an assertion subject, as described in the previous section. It is also used in an
    /// attribute query to request that the values of specific SAML attributes be returned (see Section 3.3.2.3 for
    /// more information).
    /// </summary>
    [XmlInclude(typeof(RequestedAttribute))]
    [Serializable]    
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class SamlAttribute
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Attribute";

        /// <summary>
        /// Name format "uri".
        /// </summary>
        public const string NameformatUri = "urn:oasis:names:tc:SAML:2.0:attrname-format:uri";
        
        /// <summary>
        /// Name format "basic".
        /// </summary>
        public const string NameformatBasic = "urn:oasis:names:tc:SAML:2.0:attrname-format:basic";

        #region Attributes

        /// <summary>
        /// Gets or sets XML any attribute.
        /// </summary>
        /// <value>The XML Any attribute.</value>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a URI reference representing the classification of the attribute name for purposes of interpreting the
        /// name. See Section 8.2 for some URI references that MAY be used as the value of the NameFormat
        /// attribute and their associated descriptions and processing rules. If no NameFormat value is provided,
        /// the identifier <c>urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified</c> (see Section
        /// 8.2.1) is in effect.
        /// </summary>
        [XmlAttribute("NameFormat", DataType = "anyURI")]
        public string NameFormat { get; set; }

        /// <summary>
        /// Gets or sets a string that provides a more human-readable form of the attribute's name, which may be useful in
        /// cases in which the actual Name is complex or opaque, such as an OID or a UUID. This attribute's
        /// value MUST NOT be used as a basis for formally identifying SAML attributes.
        /// </summary>
        [XmlAttribute("FriendlyName")]
        public string FriendlyName { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the attribute value.
        /// Contains a value of the attribute. If an attribute contains more than one discrete value, it is
        /// RECOMMENDED that each value appear in its own <c>&lt;AttributeValue&gt;</c> element. If more than
        /// one <c>&lt;AttributeValue&gt;</c> element is supplied for an attribute, and any of the elements have a
        /// data type assigned through <c>xsi:type</c>, then all of the <c>&lt;AttributeValue&gt;</c> elements must have
        /// the identical data type assigned.
        /// </summary>
        /// <value>The attribute value.</value>
        [XmlElement("AttributeValue", IsNullable = true, Order = 1)]
        public string[] AttributeValue { get; set; }

        #endregion
    }
}
