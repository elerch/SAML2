using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// If the requester knows the unique identifier of one or more assertions, the <c>&lt;AssertionIDRequest&gt;</c>
    /// message element can be used to request that they be returned in a &lt;Response&gt; message. The
    /// <c>&lt;saml:AssertionIDRef&gt;</c> element is used to specify each assertion to return. See Section 2.3.1 for
    /// more information on this element.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class AssertionIdRequest : RequestAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "AssertionIDRequest";

        #region Elements

        /// <summary>
        /// Gets or sets the assertion ID ref.
        /// </summary>
        /// <value>The assertion ID ref.</value>
        [XmlElement("AssertionIDRef", Namespace = Saml20Constants.Assertion, DataType = "NCName", Order = 1)]
        public string[] AssertionIdRef { get; set; }

        #endregion
    }
}
