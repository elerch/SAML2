using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// Managed NameIDRequest.
    /// </summary>
    [Serializable]    
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class ManageNameIdRequest : RequestAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "ManageNameIDRequest";

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// The name identifier and associated descriptive data (in plaintext or encrypted form) that specify the
        /// principal as currently recognized by the identity and service providers prior to this request.
        /// </summary>
        /// <value>The item.</value>
        [XmlElement("EncryptedID", typeof(EncryptedElement), Namespace = Saml20Constants.Assertion, Order = 1)]
        [XmlElement("NameID", typeof(NameId), Namespace = Saml20Constants.Assertion, Order = 1)]
        public object Item { get; set; }

        /// <summary>
        /// Gets or sets the item1.
        /// The new identifier value (in plaintext or encrypted form) to be used when communicating with the
        /// requesting provider concerning this principal, or an indication that the use of the old identifier has
        /// been terminated. In the former case, if the requester is the service provider, the new identifier MUST
        /// appear in subsequent &lt;NameID&gt; elements in the SPProvidedID attribute. If the requester is the
        /// identity provider, the new value will appear in subsequent &lt;NameID&gt; elements as the element's
        /// content.
        /// </summary>
        /// <value>The item1.</value>
        [XmlElement("NewEncryptedID", typeof(EncryptedElement), Order = 2)]
        [XmlElement("NewID", typeof(string), Order = 2)]
        [XmlElement("Terminate", typeof(Terminate), Order = 2)]
        public object Item1 { get; set; }

        #endregion
    }
}
