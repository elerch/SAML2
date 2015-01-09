using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// Contains a list of SignatureProperty instances
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class SignatureProperties
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SignatureProperties";

        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("Id", DataType = "ID")]
        public string Id { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the signature property.
        /// </summary>
        /// <value>The signature property.</value>
        [XmlElement("SignatureProperty")]
        public SignatureProperty[] SignatureProperty { get; set; }

        #endregion
    }
}
