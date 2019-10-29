using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// KeyInfo item types.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig, IncludeInSchema = false)]
    public enum KeyInfoItemType
    {
        /// <summary>
        /// Any item type.
        /// </summary>
        [XmlEnum("##any:")]
        Item,
        
        /// <summary>
        /// <c>KeyName</c> item type.
        /// </summary>
        [XmlEnum("KeyName")]
        KeyName,
        
        /// <summary>
        /// <c>KeyValue</c> item type.
        /// </summary>
        [XmlEnum("KeyValue")]
        KeyValue,
        
        /// <summary>
        /// <c>MgmtData</c> item type.
        /// </summary>
        [XmlEnum("MgmtData")]
        MgmtData,
        
        /// <summary>
        /// <c>PGPData</c> item type.
        /// </summary>
        [XmlEnum("PGPData")]
        PGPData,
        
        /// <summary>
        /// <c>RetrievalMethod</c> item type.
        /// </summary>
        [XmlEnum("RetrievalMethod")]
        RetrievalMethod,
        
        /// <summary>
        /// <c>SPKIData</c> item type.
        /// </summary>
        [XmlEnum("SPKIData")]
        SPKIData,
        
        /// <summary>
        /// <c>X509Data</c> item type.
        /// </summary>
        [XmlEnum("X509Data")]
        X509Data
    }
}
