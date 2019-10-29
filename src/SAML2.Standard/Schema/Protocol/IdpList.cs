using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The &lt;IDPList&gt; element specifies the identity providers trusted by the requester to authenticate the
    /// presenter.
    /// </summary>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class IdpList
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "IDPList";

        #region Elements

        /// <summary>
        /// Gets or sets the IDP entry.
        /// Information about a single identity provider.
        /// </summary>
        /// <value>The IDP entry.</value>
        [XmlElement("IDPEntry", Order = 1)]
        public IdpEntry[] IDPEntry { get; set; }

        /// <summary>
        /// Gets or sets the get complete.
        /// If the &lt;IDPList&gt; is not complete, using this element specifies a URI reference that can be used to
        /// retrieve the complete list. Retrieving the resource associated with the URI MUST result in an XML
        /// instance whose root element is an &lt;IDPList&gt; that does not itself contain a &lt;GetComplete&gt;
        /// element.
        /// </summary>
        /// <value>The get complete.</value>
        [XmlElement("GetComplete", DataType = "anyURI", Order = 2)]
        public string GetComplete { get; set; }

        #endregion
    }
}
