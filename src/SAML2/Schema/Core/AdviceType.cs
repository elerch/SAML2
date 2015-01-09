using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// AdviceType enumeration.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion, IncludeInSchema = false)]
    public enum AdviceType
    {
        /// <summary>
        /// Item of type any 
        /// </summary>
        [XmlEnum("##any:")]
        Item,

        /// <summary>
        /// Item of type Assertion
        /// </summary>
        [XmlEnum("Assertion")]
        Assertion,

        /// <summary>
        /// Item of type AssertionIDRef
        /// </summary>
        [XmlEnum("AssertionIDRef")]
        AssertionIDRef,

        /// <summary>
        /// Item of type AssertionURIRef
        /// </summary>
        [XmlEnum("AssertionURIRef")]
        AssertionURIRef,

        /// <summary>
        /// Item of type EncryptedAssertion
        /// </summary>
        [XmlEnum("EncryptedAssertion")]
        EncryptedAssertion,
    }
}