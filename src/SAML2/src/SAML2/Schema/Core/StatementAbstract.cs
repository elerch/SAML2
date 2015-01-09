using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The SAML20 <c>StatementAbstract</c> class. It's the base class for all statements in SAML20.
    /// </summary>
    [XmlInclude(typeof(AttributeStatement))]
    [XmlIncludeAttribute(typeof(AuthzDecisionStatement))]
    [XmlIncludeAttribute(typeof(AuthnStatement))]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public abstract class StatementAbstract
    {        
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Statement";
    }
}