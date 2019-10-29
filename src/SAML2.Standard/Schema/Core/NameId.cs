using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The NameIDType complex type is used when an element serves to represent an entity by a string-valued
    /// name. It is a more restricted form of identifier than the &lt;BaseID&gt; element and is the type underlying both
    /// the &lt;NameID&gt; and &lt;Issuer&gt; elements.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class NameId
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "NameID";

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public string Value { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the format.
        /// A URI reference representing the classification of string-based identifier information. See Section
        /// 8.3 for the SAML-defined URI references that MAY be used as the value of the Format attribute
        /// and their associated descriptions and processing rules. Unless otherwise specified by an element
        /// based on this type, if no Format value is provided, then the value
        /// <c>urn:oasis:names:tc:SAML:1.0:nameid-format:unspecified</c> (see Section 8.3.1) is in
        /// effect.
        /// When a Format value other than one specified in Section 8.3 is used, the content of an element
        /// of this type is to be interpreted according to the definition of that format as provided outside of this
        /// specification. If not otherwise indicated by the definition of the format, issues of anonymity,
        /// pseudonym, and the persistence of the identifier with respect to the asserting and relying parties
        /// are implementation-specific.
        /// </summary>
        /// <value>The format.</value>
        [XmlAttribute("Format", DataType = "anyURI")]
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the name qualifier.
        /// The security or administrative domain that qualifies the name. This attribute provides a means to
        /// federate names from disparate user stores without collision.
        /// </summary>
        /// <value>The name qualifier.</value>
        [XmlAttribute("NameQualifier")]
        public string NameQualifier { get; set; }

        /// <summary>
        /// Gets or sets the SP name qualifier.
        /// Further qualifies a name with the name of a service provider or affiliation of providers. This
        /// attribute provides an additional means to federate names on the basis of the relying party or
        /// parties.
        /// </summary>
        /// <value>The SP name qualifier.</value>
        [XmlAttribute("SPNameQualifier")]
        public string SPNameQualifier { get; set; }
        
        /// <summary>
        /// Gets or sets the SP provided ID.
        /// A name identifier established by a service provider or affiliation of providers for the entity, if
        /// different from the primary name identifier given in the content of the element. This attribute
        /// provides a means of integrating the use of SAML with existing identifiers already in use by a
        /// service provider. For example, an existing identifier can be "attached" to the entity using the Name
        /// Identifier Management protocol defined in Section 3.6.
        /// </summary>
        /// <value>The SP provided ID.</value>
        [XmlAttribute("SPProvidedID")]
        public string SPProvidedID { get; set; }

        #endregion
    }
}
