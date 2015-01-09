using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The <c>&lt;Scoping&gt;</c> element specifies the identity providers trusted by the requester to authenticate the
    /// presenter, as well as limitations and context related to a proxy of the <c>&lt;AuthnRequest&gt;</c> message to
    /// subsequent identity providers by the responder.
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class Scoping
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Scoping";

        #region Attributes

        /// <summary>
        /// Gets or sets the proxy count.
        /// Specifies the number of a proxy indirections permissible between the identity provider that receives
        /// this <c>&lt;AuthnRequest&gt;</c> and the identity provider who ultimately authenticates the principal. A count of
        /// zero permits no proxy, while omitting this attribute expresses no such restriction.
        /// </summary>
        /// <value>The proxy count.</value>
        [XmlAttribute("ProxyCount", DataType = "nonNegativeInteger")]
        public string ProxyCount { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the IDP list.
        /// An advisory list of identity providers and associated information that the requester deems acceptable
        /// to respond to the request.
        /// </summary>
        /// <value>The IDP list.</value>
        [XmlElement("IDPList", Order = 1)]
        public IdpList IdpList { get; set; }

        /// <summary>
        /// Gets or sets the requester ID.
        /// Identifies the set of requesting entities on whose behalf the requester is acting. Used to communicate
        /// the chain of requesters when a proxy occurs, as described in Section 3.4.1.5. See Section 8.3.6 for a
        /// description of entity identifiers.
        /// </summary>
        /// <value>The requester ID.</value>
        [XmlElement("RequesterID", DataType = "anyURI", Order = 2)]
        public string[] RequesterId { get; set; }

        #endregion
    }
}
