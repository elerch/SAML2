using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The <c>&lt;AuthnQuery&gt;</c> message element is used to make the query "What assertions containing
    /// authentication statements are available for this subject?" A successful <c>&lt;Response&gt;</c> will contain one or
    /// more assertions containing authentication statements.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class AuthnQuery : SubjectQueryAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AuthnQuery";

        #region Attributes

        /// <summary>
        /// Gets or sets the index of the session.
        /// If present, specifies a filter for possible responses. Such a query asks the question "What assertions
        /// containing authentication statements do you have for this subject within the context of the supplied
        /// session information?"
        /// </summary>
        /// <value>The index of the session.</value>
        [XmlAttribute]
        public string SessionIndex { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the requested authentication context.
        /// If present, specifies a filter for possible responses. Such a query asks the question "What assertions
        /// containing authentication statements do you have for this subject that satisfy the authentication
        /// context requirements in this element?"
        /// </summary>
        /// <value>The requested authentication context.</value>
        [XmlElement("RequestedAuthnContext", Order = 1)]
        public RequestedAuthnContext RequestedAuthnContext { get; set; }

        #endregion
    }
}
