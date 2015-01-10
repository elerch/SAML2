using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// Object is an optional element that may occur one or more times. When present, this element may contain 
    /// any data. The Object element may include optional MIME type, ID, and encoding attributes. 
    /// </para>
    /// <para>
    /// The Object's Encoding attributed may be used to provide a URI that identifies the method by which the 
    /// object is encoded (e.g., a binary file). 
    /// </para>
    /// <para>
    /// The MimeType attribute is an optional attribute which describes the data within the Object (independent 
    /// of its encoding). This is a string with values defined by [MIME]. For example, if the Object contains 
    /// base64 encoded PNG, the Encoding may be specified as '<c>base64</c>' and the MimeType as '<c>image/png</c>'. This 
    /// attribute is purely advisory; no validation of the MimeType information is required by this specification. 
    /// Applications which require normative type and encoding information for signature validation should specify 
    /// Transforms with well defined resulting types and/or encodings. 
    /// </para>
    /// <para>
    /// The Object's Id is commonly referenced from a Reference in SignedInfo, or Manifest. This element is 
    /// typically used for enveloping signatures where the object being signed is to be included in the signature 
    /// element. The digest is calculated over the entire Object element including start and end tags. 
    /// </para>
    /// <para>
    /// Note, if the application wishes to exclude the &lt;Object&gt; tags from the digest calculation the Reference 
    /// must identify the actual data object (easy for XML documents) or a transform must be used to remove the 
    /// Object tags (likely where the data object is non-XML). Exclusion of the object tags may be desired for 
    /// cases where one wants the signature to remain valid if the data object is moved from inside a signature 
    /// to outside the signature (or vice versa), or where the content of the Object is an encoding of an original
    /// binary document and it is desired to extract and decode so as to sign the original bitwise representation.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class ObjectType
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Object";

        #region Attributes

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        [XmlAttribute("Encoding", DataType = "anyURI")]
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("Id", DataType = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        [XmlAttribute("MimeType")]
        public string MimeType { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets any elements.
        /// </summary>
        /// <value>Any elements.</value>
        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any { get; set; }

        #endregion
    }
}
