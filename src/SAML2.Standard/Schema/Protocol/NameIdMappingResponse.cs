using System;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The recipient of a &lt;NameIDMappingRequest&gt; message MUST respond with a
    /// &lt;NameIDMappingResponse&gt; message.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class NameIdMappingResponse : StatusResponse
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "NameIDMappingResponse";

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// The identifier and associated attributes that specify the principal in the manner requested, usually in
        /// encrypted form
        /// </summary>
        /// <value>The item.</value>
        [XmlElement("EncryptedID", typeof(EncryptedElement), Namespace = Saml20Constants.Assertion, Order = 1)]
        [XmlElement("NameID", typeof(NameId), Namespace = Saml20Constants.Assertion, Order = 1)]
        public object Item { get; set; }

        #endregion
    }
}
