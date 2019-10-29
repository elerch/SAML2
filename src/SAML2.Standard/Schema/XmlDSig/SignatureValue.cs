using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// The SignatureValue element contains the actual value of the digital signature; it is always encoded using 
    /// base64 [MIME]. While we identify two SignatureMethod algorithms, one mandatory and one optional to 
    /// implement, user specified algorithms may be used as well. 
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class SignatureValue
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SignatureValue";

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText(DataType = "base64Binary")]
        public byte[] Value { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }

        #endregion
    }
}
