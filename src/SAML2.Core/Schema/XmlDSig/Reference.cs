using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// Reference is an element that may occur one or more times. It specifies a digest algorithm and digest 
    /// value, and optionally an identifier of the object being signed, the type of the object, and/or a list 
    /// of transforms to be applied prior to digesting. The identification (URI) and transforms describe how the 
    /// digested content (i.e., the input to the digest method) was created. The Type attribute facilitates the 
    /// processing of referenced data. For example, while this specification makes no requirements over external 
    /// data, an application may wish to signal that the referent is a Manifest. An optional ID attribute permits 
    /// a Reference to be referenced from elsewhere. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class Reference
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Reference";

        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("Id", DataType = "ID")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        [XmlAttribute("URI", DataType = "anyURI")]
        public string URI { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlAttribute("Type", DataType = "anyURI")]
        public string Type { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the transforms.
        /// </summary>
        /// <value>The transforms.</value>
        [XmlArrayItem("Transform", IsNullable = false)]
        public Transform[] Transforms { get; set; }

        /// <summary>
        /// Gets or sets the digest method.
        /// </summary>
        /// <value>The digest method.</value>
        [XmlElement("DigestMethod")]
        public DigestMethod DigestMethod { get; set; }

        /// <summary>
        /// Gets or sets the digest value.
        /// </summary>
        /// <value>The digest value.</value>
        [XmlElement("DigestValue", DataType = "base64Binary")]
        public byte[] DigestValue { get; set; }

        #endregion
    }
}
