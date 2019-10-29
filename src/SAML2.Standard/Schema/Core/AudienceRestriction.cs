using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;AudienceRestriction&gt; element specifies that the assertion is addressed to one or more
    /// specific audiences identified by &lt;Audience&gt; elements. Although a SAML relying party that is outside the
    /// audiences specified is capable of drawing conclusions from an assertion, the SAML asserting party
    /// explicitly makes no representation as to accuracy or trustworthiness to such a party. It contains the
    /// following element:
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class AudienceRestriction : ConditionAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AudienceRestriction";

        #region Elements

        /// <summary>
        /// Gets or sets the audience.
        /// A URI reference that identifies an intended audience. The URI reference MAY identify a document
        /// that describes the terms and conditions of audience membership. It MAY also contain the unique
        /// identifier URI from a SAML name identifier that describes a system entity
        /// </summary>
        /// <value>The audience.</value>
        [XmlElement("Audience", DataType = "anyURI", Order = 1)]
        public List<string> Audience { get; set; }

        #endregion
    }
}
