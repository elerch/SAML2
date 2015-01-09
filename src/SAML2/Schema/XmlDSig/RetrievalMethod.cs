using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// A RetrievalMethod element within KeyInfo is used to convey a reference to KeyInfo information that is 
    /// stored at another location. For example, several signatures in a document might use a key verified by 
    /// an X.509v3 certificate chain appearing once in the document or remotely outside the document; each 
    /// signature's KeyInfo can reference this chain using a single RetrievalMethod element instead of including 
    /// the entire chain with a sequence of X509Certificate elements. 
    /// </para>
    /// <para>
    /// RetrievalMethod uses the same syntax and dereferencing behavior as Reference's URI (section 4.3.3.1) and 
    /// The Reference Processing Model (section 4.3.3.2) except that there is no DigestMethod or DigestValue 
    /// child elements and presence of the URI is mandatory. 
    /// </para>
    /// <para>
    /// Type is an optional identifier for the type of data to be retrieved. The result of dereferencing a 
    /// RetrievalMethod Reference for all KeyInfo types defined by this specification (section 4.4) with a 
    /// corresponding XML structure is an XML element or document with that element as the root. The 
    /// rawX509Certificate KeyInfo (for which there is no XML structure) returns a binary X509 certificate. 
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class RetrievalMethod
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "RetrievalMethod";

        #region Attributes

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        [XmlAttribute("URI", DataType = "anyURI")]
        public string Uri { get; set; }

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

        #endregion
    }
}