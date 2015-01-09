using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The &lt;AttributeQuery&gt; element is used to make the query "Return the requested attributes for this
    /// subject." A successful response will be in the form of assertions containing attribute statements, to the
    /// extent allowed by policy.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class AttributeQuery : SubjectQueryAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AttributeQuery";

        #region Elements

        /// <summary>
        /// Gets or sets the attribute.
        /// Each <c>&lt;saml:Attribute&gt;</c> element specifies an attribute whose value(s) are to be returned. If no
        /// attributes are specified, it indicates that all attributes allowed by policy are requested. If a given
        /// <c>&lt;saml:Attribute&gt;</c> element contains one or more <c>&lt;saml:AttributeValue&gt;</c> elements, then if
        /// that attribute is returned in the response, it MUST NOT contain any values that are not equal to the
        /// values specified in the query. In the absence of equality rules specified by particular profiles or
        /// attributes, equality is defined as an identical XML representation of the value
        /// </summary>
        /// <value>The attribute.</value>
        [XmlElement("Attribute", Namespace = Saml20Constants.Assertion, Order = 1)]
        public SamlAttribute[] SamlAttribute { get; set; }

        #endregion
    }
}
