using System;
using System.Xml.Serialization;
using SAML2.Schema.Protocol;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;SubjectConfirmation&gt; element provides the means for a relying party to verify the
    /// correspondence of the subject of the assertion with the party with whom the relying party is
    /// communicating.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class SubjectConfirmation
    {
        /// <summary>
        /// The BEARER_METHOD constant
        /// </summary>
        public const string BearerMethod = "urn:oasis:names:tc:SAML:2.0:cm:bearer";

        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SubjectConfirmation";

        #region Attributes

        /// <summary>
        /// Gets or sets the method.
        /// A URI reference that identifies a protocol or mechanism to be used to confirm the subject. URI
        /// references identifying SAML-defined confirmation methods are currently defined in the SAML profiles
        /// specification [SAMLProf]. Additional methods MAY be added by defining new URIs and profiles or by
        /// private agreement.
        /// </summary>
        /// <value>The method.</value>
        [XmlAttribute("Method", DataType = "anyURI")]
        public string Method { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the item.
        /// Identifies the entity expected to satisfy the enclosing subject confirmation requirements.
        /// Valid elements are &lt;BaseID&gt;, &lt;NameID&gt;, or &lt;EncryptedID&gt; 
        /// </summary>
        /// <value>The item.</value>
        [XmlElement("BaseID", typeof(BaseIdAbstract), Order = 1)]
        [XmlElement("EncryptedID", typeof(EncryptedElement), Order = 1)]
        [XmlElement("NameID", typeof(NameId), Order = 1)]
        public object Item { get; set; }

        /// <summary>
        /// Gets or sets the subject confirmation data.
        /// Additional confirmation information to be used by a specific confirmation method. For example, typical
        /// content of this element might be a &lt;ds:KeyInfo&gt; element as defined in the XML Signature Syntax
        /// and Processing specification [XMLSig], which identifies a cryptographic key (See also Section
        /// 2.4.1.3). Particular confirmation methods MAY define a schema type to describe the elements,
        /// attributes, or content that may appear in the &lt;SubjectConfirmationData&gt; element.
        /// </summary>
        /// <value>The subject confirmation data.</value>
        [XmlElement("SubjectConfirmationData", Order = 2)]
        public SubjectConfirmationData SubjectConfirmationData { get; set; }

        #endregion
    }
}
