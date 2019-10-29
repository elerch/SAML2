using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// Authentication context type enumeration.
    /// </summary>
    [Serializable]
    [XmlTypeAttribute(Namespace = Saml20Constants.Assertion, IncludeInSchema = false)]
    public enum AuthnContextType
    {
        /// <summary>
        /// Item of type <c>AuthnContextClassRef</c>
        /// </summary>
        [XmlEnum("AuthnContextClassRef")]
        AuthnContextClassRef,

        /// <summary>
        /// Item of type <c>AuthnContextDecl</c>
        /// </summary>
        [XmlEnum("AuthnContextDecl")]
        AuthnContextDecl,

        /// <summary>
        /// Item of type <c>AuthnContextDeclRef</c>
        /// </summary>
        [XmlEnum("AuthnContextDeclRef")]
        AuthnContextDeclRef,
    }
}