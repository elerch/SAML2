using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The SAML ConditionAbstract class.
    /// Serves as an extension point for new conditions.
    /// </summary>
    [XmlInclude(typeof(ProxyRestriction))]
    [XmlInclude(typeof(OneTimeUse))]
    [XmlInclude(typeof(AudienceRestriction))]
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public abstract class ConditionAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Condition";
    }
}