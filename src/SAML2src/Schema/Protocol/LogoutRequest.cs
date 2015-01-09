using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;
using SAML2.Utils;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// A session participant or session authority sends a &lt;LogoutRequest&gt; message to indicate that a session
    /// has been terminated.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class LogoutRequest : RequestAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "LogoutRequest";

        /// <summary>
        /// Specifies that the message is being sent because the principal wishes to terminate the indicated session.
        /// </summary>
        public const string ReasonUser = "urn:oasis:names:tc:SAML:2.0:logout:user";

        /// <summary>
        /// Specifies that the message is being sent because an administrator wishes to terminate the indicated session for 
        /// the principal.
        /// </summary>
        public const string ReasonAdmin = "urn:oasis:names:tc:SAML:2.0:logout:admin";

        /// <summary>
        /// Gets or sets NotOnOrAfter.
        /// </summary>
        /// <value>The not on or after.</value>
        [XmlIgnore]
        public DateTime? NotOnOrAfter { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the issue instant string.
        /// </summary>
        /// <value>The issue instant string.</value>
        [XmlAttribute("NotOnOrAfter")]
        public string NotOnOrAfterString
        {
            get { return NotOnOrAfter.HasValue ? Saml20Utils.ToUtcString(NotOnOrAfter.Value) : null; }
            set { NotOnOrAfter = string.IsNullOrEmpty(value) ? (DateTime?)null : Saml20Utils.FromUtcString(value); }
        }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>The reason.</value>
        [XmlAttribute("Reason")]
        public string Reason { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// The identifier and associated attributes (in plaintext or encrypted form) that specify the principal as
        /// currently recognized by the identity and service providers prior to this request.
        /// </summary>
        /// <value>The item.</value>
        [XmlElement("BaseID", typeof(BaseIdAbstract), Namespace = Saml20Constants.Assertion, Order = 1)]
        [XmlElement("EncryptedID", typeof(EncryptedElement), Namespace = Saml20Constants.Assertion, Order = 1)]
        [XmlElement("NameID", typeof(NameId), Namespace = Saml20Constants.Assertion, Order = 1)]
        public object Item { get; set; }

        /// <summary>
        /// Gets or sets the index of the session.
        /// </summary>
        /// <value>The index of the session.</value>
        [XmlElement("SessionIndex", Order = 2)]
        public string[] SessionIndex { get; set; }

        #endregion
    }
}
