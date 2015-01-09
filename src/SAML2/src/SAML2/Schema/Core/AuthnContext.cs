using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The <c>&lt;AuthnContext&gt;</c> element specifies the context of an authentication event. The element can contain
    /// an authentication context class reference, an authentication context declaration or declaration reference,
    /// or both.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class AuthnContext
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "AuthnContext";

        #region Elements

        /// <summary>
        /// Gets or sets the authenticating authority.
        /// Zero or more unique identifiers of authentication authorities that were involved in the authentication of
        /// the principal (not including the assertion issuer, who is presumed to have been involved without being
        /// explicitly named here).
        /// </summary>
        /// <value>The authenticating authority.</value>
        [XmlElement("AuthenticatingAuthority", DataType = "anyURI", Order = 2)]
        public string[] AuthenticatingAuthority { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// Items may be of types: <c>AuthnContextClassRef</c>, <c>AuthnContextDecl</c> and <c>AuthnContextDeclRef</c>
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("AuthnContextClassRef", typeof(string), DataType = "anyURI", Order = 1)]
        [XmlElement("AuthnContextDecl", typeof(object), Order = 1)]
        [XmlElement("AuthnContextDeclRef", typeof(string), DataType = "anyURI", Order = 1)]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items { get; set; }

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
