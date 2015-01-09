using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Utils;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// KeyInfo is an optional element that enables the recipient(s) to obtain the key needed to validate the 
    /// signature.  KeyInfo may contain keys, names, certificates and other public key management information, 
    /// such as in-band key distribution or key agreement data. This specification defines a few simple types 
    /// but applications may extend those types or all together replace them with their own key identification 
    /// and exchange semantics using the XML namespace facility. [XML-ns] However, questions of trust of such 
    /// key information (e.g., its authenticity or  strength) are out of scope of this specification and left 
    /// to the application. 
    /// </para>
    /// <para>
    /// If KeyInfo is omitted, the recipient is expected to be able to identify the key based on application 
    /// context. Multiple declarations within KeyInfo refer to the same key. While applications may define and 
    /// use any mechanism they choose through inclusion of elements from a different namespace, compliant 
    /// versions MUST implement KeyValue (section 4.4.2) and SHOULD implement RetrievalMethod (section 4.4.3). 
    /// </para>
    /// <para>
    /// The schema/DTD specifications of many of KeyInfo's children (e.g., PGPData, SPKIData, X509Data) permit 
    /// their content to be extended/complemented with elements from another namespace. This may be done only 
    /// if it is safe to ignore these extension elements while claiming support for the types defined in this 
    /// specification. Otherwise, external elements, including alternative structures to those defined by this 
    /// specification, MUST be a child of KeyInfo. For example, should a complete XML-PGP standard be defined, 
    /// its root element MUST be a child of KeyInfo. (Of course, new structures from external namespaces can 
    /// incorporate elements from the dsig namespace via features of the type definition language.
    /// </para>
    /// </summary>
    [Serializable]    
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class KeyInfo
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "KeyInfo";

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [XmlText]
        public string[] Text { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Items are of types:
        /// <c>KeyName</c>, <c>KeyValue</c>, <c>MgmtData</c>, <c>PGPData</c>, <c>RetrievalMethod</c>, <c>SPKIData</c>, <c>X509Data</c>
        /// </summary>
        /// <value>The items.</value>
        [XmlAnyElement]
        [XmlElement("KeyName", typeof(string))]
        [XmlElement(KeyValue.ElementName, typeof(KeyValue))]
        [XmlElement("MgmtData", typeof(string))]
        [XmlElement(PgpData.ElementName, typeof(PgpData))]
        [XmlElement(RetrievalMethod.ElementName, typeof(RetrievalMethod))]
        [XmlElement(SpkiData.ElementName, typeof(SpkiData))]
        [XmlElement(X509Data.ElementName, typeof(X509Data))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items { get; set; }

        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public KeyInfoItemType[] ItemsElementName { get; set; }

        #endregion

        /// <summary>
        /// An implicit conversion between our Xml Serialization class, and the .NET framework's built-in version of KeyInfo.
        /// </summary>
        /// <param name="ki">The key info.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator System.Security.Cryptography.Xml.KeyInfo(KeyInfo ki)
        {
            var result = new System.Security.Cryptography.Xml.KeyInfo();
            var doc = new XmlDocument();
            doc.LoadXml(Serialization.SerializeToXmlString(ki));
            if (doc.DocumentElement != null)
            {
                result.LoadXml(doc.DocumentElement);
            }

            return result;
        }
    }
}
