using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// The Manifest element provides a list of References. The difference from the list in SignedInfo is that 
    /// it is application defined which, if any, of the digests are actually checked against the objects referenced 
    /// and what to do if the object is inaccessible or the digest compare fails. If a Manifest is pointed to from 
    /// SignedInfo, the digest over the Manifest itself will be checked by the core signature validation behavior. 
    /// The digests within such a Manifest are checked at the application's discretion. If a Manifest is referenced 
    /// from another Manifest, even the overall digest of this two level deep Manifest might not be checked. 
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class Manifest
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Manifest";

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
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        [XmlElement("Reference")]
        public Reference[] Reference { get; set; }

        #endregion
    }
}
