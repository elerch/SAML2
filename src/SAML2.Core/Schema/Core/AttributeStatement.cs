using System;
using System.Xml.Serialization;
using SAML2.Schema.Protocol;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;AttributeStatement&gt; element describes a statement by the SAML authority asserting that the
    /// assertion subject is associated with the specified attributes. Assertions containing
    /// &lt;AttributeStatement&gt; elements MUST contain a &lt;Subject&gt; element.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class AttributeStatement : StatementAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AttributeStatement";

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Items may be of type Attribute and EncryptedAttribute
        /// </summary>
        /// <value>The items.</value>
        [XmlElement(SamlAttribute.ElementName, typeof(SamlAttribute), Order = 1)]
        [XmlElement("EncryptedAttribute", typeof(EncryptedElement), Order = 1)]
        public object[] Items { get; set; }

        #endregion
    }
}