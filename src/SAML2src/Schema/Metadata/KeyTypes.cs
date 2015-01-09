using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// Key types enumeration.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    public enum KeyTypes
    {
        /// <summary>
        /// Encryption key type.
        /// </summary>
        [XmlEnum("encryption")]
        Encryption,

        /// <summary>
        /// Signing key type.
        /// </summary>
        [XmlEnum("signing")]
        Signing
    }
}