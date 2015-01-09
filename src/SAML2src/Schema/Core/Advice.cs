using System;
using System.Xml.Serialization;
using SAML2.Schema.Protocol;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;Advice&gt; element contains any additional information that the SAML authority wishes to provide.
    /// This information MAY be ignored by applications without affecting either the semantics or the validity of
    /// the assertion.
    /// </summary>
    /// <remarks>
    /// Advice is optional, and there are only implicit demands on the reference types.
    /// We do not use it (yet) and let it pass without validation.
    /// </remarks>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]    
    public class Advice
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Advice";

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Items may be of types: <c>Assertion</c>, <c>AssertionIDRef</c>, <c>AssertionURIRef</c> and <c>EncryptedAssertion</c>
        /// </summary>
        /// <value>The items.</value>
        [XmlAnyElement]
        [XmlElement(Assertion.ElementName, typeof(Assertion), Order = 1)]
        [XmlElement("AssertionIDRef", typeof(string), DataType = "NCName", Order = 1)]
        [XmlElement("AssertionURIRef", typeof(string), DataType = "anyURI", Order = 1)]
        [XmlElement("EncryptedAssertion", typeof(EncryptedElement), Order = 1)]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items { get; set; }

        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public AdviceType[] ItemsElementName { get; set; }

        #endregion
    }
}
