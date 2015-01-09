using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The <c>&lt;AuthzDecisionStatement&gt;</c> element describes a statement by the SAML authority asserting that
    /// a request for access by the assertion subject to the specified resource has resulted in the specified
    /// authorization decision on the basis of some optionally specified evidence. Assertions containing
    /// <c>&lt;AuthzDecisionStatement&gt;</c> elements MUST contain a <c>&lt;Subject&gt;</c> element.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class AuthzDecisionStatement : StatementAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AuthzDecisionStatement";

        #region Attributes

        /// <summary>
        /// Gets or sets the decision.
        /// The decision rendered by the SAML authority with respect to the specified resource. The value is of
        /// the DecisionType simple type.
        /// </summary>
        /// <value>The decision.</value>
        [XmlAttribute("Decision")]
        public DecisionType Decision { get; set; }

        /// <summary>
        /// Gets or sets the resource.
        /// A URI reference identifying the resource to which access authorization is sought. This attribute MAY
        /// have the value of the empty URI reference (""), and the meaning is defined to be "the start of the
        /// current document", as specified by IETF RFC 2396 [RFC 2396] Section 4.2.
        /// </summary>
        /// <value>The resource.</value>
        [XmlAttribute("Resource", DataType = "anyURI")]
        public string Resource { get; set; }
        
        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the action.
        /// The set of actions authorized to be performed on the specified resource.
        /// </summary>
        /// <value>The action.</value>
        [XmlElement("Action", Order = 1)]
        public Action[] Action { get; set; }

        /// <summary>
        /// Gets or sets the evidence.
        /// A set of assertions that the SAML authority relied on in making the decision.
        /// </summary>
        /// <value>The evidence.</value>
        [XmlElement("Evidence", Order = 2)]
        public Evidence Evidence { get; set; }

        #endregion
    }
}
