using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// <para>
    /// The <c>&lt;AuthzDecisionQuery&gt;</c> element is used to make the query "Should these actions on this resource
    /// be allowed for this subject, given this evidence?" A successful response will be in the form of assertions
    /// containing authorization decision statements.
    /// </para>
    /// <para>
    /// Note: The <c>&lt;AuthzDecisionQuery&gt;</c> feature has been frozen as of SAML V2.0, with no
    /// future enhancements planned. Users who require additional functionality may want to
    /// consider the extensible Access Control Markup Language [XACML], which offers
    /// enhanced authorization decision features.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class AuthzDecisionQuery : SubjectQueryAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AuthzDecisionQuery";

        #region Attributes

        /// <summary>
        /// Gets or sets the resource.
        /// A URI reference indicating the resource for which authorization is requested.
        /// </summary>
        /// <value>The resource.</value>
        [XmlAttribute("Resource", DataType = "anyURI")]
        public string Resource { get; set; }
        
        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the action.
        /// The actions for which authorization is requested.
        /// </summary>
        /// <value>The action.</value>
        [XmlElement("Action", Namespace = Saml20Constants.Assertion, Order = 1)]
        public Core.Action[] Action { get; set; }

        /// <summary>
        /// Gets or sets the evidence.
        /// A set of assertions that the SAML authority MAY rely on in making its authorization decision
        /// </summary>
        /// <value>The evidence.</value>
        [XmlElement("Evidence", Namespace = Saml20Constants.Assertion, Order = 2)]
        public Evidence Evidence { get; set; }

        #endregion
    }
}
