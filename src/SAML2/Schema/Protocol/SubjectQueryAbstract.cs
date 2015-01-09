using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// is an extension point that allows new SAML queries to be
    /// defined that specify a single SAML subject.
    /// </summary>
    [XmlInclude(typeof(AuthzDecisionQuery))]
    [XmlInclude(typeof(AttributeQuery))]
    [XmlInclude(typeof(AuthnQuery))]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public abstract class SubjectQueryAbstract : RequestAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SubjectQuery";

        #region Elements

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement("Subject", Namespace = Saml20Constants.Assertion, Order = 1)]
        public Subject Subject { get; set; }

        #endregion
    }
}
