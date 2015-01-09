using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// PGP item type.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig, IncludeInSchema = false)]
    public enum PgpItemType
    {
        /// <summary>
        /// Any item type.
        /// </summary>
        [XmlEnum("##any:")]
        Item,

        /// <summary>
        /// <c>PgpKeyId</c> item type.
        /// </summary>
        [XmlEnum("PGPKeyID")]
        PgpKeyId,

        /// <summary>
        /// <c>PgpKeyPacket</c> item type.
        /// </summary>
        [XmlEnum("PGPKeyPacket")]
        PgpKeyPacket,
    }
}
