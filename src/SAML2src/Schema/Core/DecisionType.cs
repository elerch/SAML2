using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// Decision types
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    public enum DecisionType
    {
        /// <summary>
        /// Permit decision type
        /// </summary>
        [XmlEnum("Permit")]
        Permit,

        /// <summary>
        /// Deny decision type
        /// </summary>
        [XmlEnum("Deny")]
        Deny,

        /// <summary>
        /// Indeterminate decision type
        /// </summary>
        [XmlEnum("Indeterminate")]
        Indeterminate,
    }
}