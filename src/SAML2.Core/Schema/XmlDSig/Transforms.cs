using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// Holds a list of transform classes
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class Transforms
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Transforms";

        #region Elements

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        [XmlElement("Transform")]
        public Transform[] Transform { get; set; }

        #endregion
    }
}
