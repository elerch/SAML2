using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// Contact type enumeration.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    public enum ContactType
    {
        /// <summary>
        /// technical contact type.
        /// </summary>
        [XmlEnum("technical")]
        Technical,
        
        /// <summary>
        /// support contact type.
        /// </summary>
        [XmlEnum("support")]
        Support,
        
        /// <summary>
        /// administrative contact type.
        /// </summary>
        [XmlEnum("administrative")]
        Administrative,
        
        /// <summary>
        /// billing contact type.
        /// </summary>
        [XmlEnum("billing")]
        Billing,
        
        /// <summary>
        /// other contact type.
        /// </summary>
        [XmlEnum("other")]
        Other
    }
}