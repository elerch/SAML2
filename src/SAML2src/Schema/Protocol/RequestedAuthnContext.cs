using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The <c>&lt;RequestedAuthnContext&gt;</c> element specifies the authentication context requirements of
    /// authentication statements returned in response to a request or query.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class RequestedAuthnContext
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "RequestedAuthnContext";

        /// <summary>
        /// Gets or sets a value indicating whether [comparison specified].
        /// </summary>
        /// <value><c>true</c> if [comparison specified]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool ComparisonSpecified { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the comparison.
        /// Specifies the comparison method used to evaluate the requested context classes or statements, one
        /// of "exact", "minimum", "maximum", or "better". The default is "exact".
        /// If Comparison is set to "exact" or omitted, then the resulting authentication context in the authentication
        /// statement MUST be the exact match of at least one of the authentication contexts specified.
        /// If Comparison is set to "minimum", then the resulting authentication context in the authentication
        /// statement MUST be at least as strong (as deemed by the responder) as one of the authentication
        /// contexts specified.
        /// If Comparison is set to "better", then the resulting authentication context in the authentication
        /// statement MUST be stronger (as deemed by the responder) than any one of the authentication contexts
        /// specified.
        /// If Comparison is set to "maximum", then the resulting authentication context in the authentication
        /// statement MUST be as strong as possible (as deemed by the responder) without exceeding the strength
        /// of at least one of the authentication contexts specified.
        /// </summary>
        /// <value>The comparison.</value>
        [XmlAttribute("Comparison")]
        public AuthnContextComparisonType Comparison { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Specifies one or more URI references identifying authentication context classes or declarations.
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("AuthnContextClassRef", typeof(string), Namespace = Saml20Constants.Assertion, DataType = "anyURI", Order = 1)]
        [XmlElement("AuthnContextDeclRef", typeof(string), Namespace = Saml20Constants.Assertion, DataType = "anyURI", Order = 1)]
        [XmlChoiceIdentifier("ItemsElementName")]
        public string[] Items { get; set; }

        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public AuthnContextType[] ItemsElementName { get; set; }

        #endregion
    }
}