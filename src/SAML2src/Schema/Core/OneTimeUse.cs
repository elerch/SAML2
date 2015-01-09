using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// <para>
    /// In general, relying parties may choose to retain assertions, or the information they contain in some other
    /// form, for reuse. The <c>&lt;OneTimeUse&gt;</c> condition element allows an authority to indicate that the information
    /// in the assertion is likely to change very soon and fresh information should be obtained for each use. An
    /// example would be an assertion containing an <c>&lt;AuthzDecisionStatement&gt;</c> which was the result of a
    /// policy which specified access control which was a function of the time of day.
    /// </para>
    /// <para>
    /// If system clocks in a distributed environment could be precisely synchronized, then this requirement could
    /// be met by careful use of the validity interval. However, since some clock skew between systems will
    /// always be present and will be combined with possible transmission delays, there is no convenient way for
    /// the issuer to appropriately limit the lifetime of an assertion without running a substantial risk that it will
    /// already have expired before it arrives.
    /// </para>
    /// <para>
    /// The <c>&lt;OneTimeUse&gt;</c> element indicates that the assertion SHOULD be used immediately by the relying
    /// party and MUST NOT be retained for future use. Relying parties are always free to request a fresh
    /// assertion for every use. However, implementations that choose to retain assertions for future use MUST
    /// observe the <c>&lt;OneTimeUse&gt;</c> element. This condition is independent from the NotBefore and
    /// NotOnOrAfter condition information.
    /// </para>
    /// <para>
    /// To support the single use constraint, a relying party should maintain a cache of the assertions it has
    /// processed containing such a condition. Whenever an assertion with this condition is processed, the cache
    /// should be checked to ensure that the same assertion has not been previously received and processed by
    /// the relying party.
    /// </para>
    /// <para>
    /// A SAML authority MUST NOT include more than one <c>&lt;OneTimeUse&gt;</c> element within a <c>&lt;Conditions&gt;</c>
    /// element of an assertion.
    /// </para>
    /// <para>
    /// For the purposes of determining the validity of the <c>&lt;Conditions&gt;</c> element, the <c>&lt;OneTimeUse&gt;</c> is
    /// considered to always be valid. That is, this condition does not affect validity but is a condition on use.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class OneTimeUse : ConditionAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "OneTimeUse";
    }
}