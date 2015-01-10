using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SAML2.Utils;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The SAML20 Conditions class
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class Conditions
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Conditions";

        /// <summary>
        /// Gets or sets the not before.
        /// Specifies the earliest time instant at which the assertion is valid. The time value is encoded in UTC
        /// </summary>
        /// <value>The not before.</value>
        [XmlIgnore]
        public DateTime? NotBefore { get; set; }

        /// <summary>
        /// Gets or sets the not on or after.
        /// Specifies the time instant at which the assertion has expired. The time value is encoded in UTC.
        /// </summary>
        /// <value>The not on or after.</value>
        [XmlIgnore]
        public DateTime? NotOnOrAfter { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the not before string.
        /// </summary>
        /// <value>The not before string.</value>
        [XmlAttribute("NotBefore")]
        public string NotBeforeString
        {
            get { return NotBefore.HasValue ? Saml20Utils.ToUtcString(NotBefore.Value) : null; }
            set { NotBefore = string.IsNullOrEmpty(value) ? (DateTime?)null : Saml20Utils.FromUtcString(value); }
        }

        /// <summary>
        /// Gets or sets the not on or after string.
        /// </summary>
        /// <value>The not on or after string.</value>
        [XmlAttribute("NotOnOrAfter")]
        public string NotOnOrAfterString
        {
            get { return NotOnOrAfter.HasValue ? Saml20Utils.ToUtcString(NotOnOrAfter.Value) : null; }
            set { NotOnOrAfter = string.IsNullOrEmpty(value) ? (DateTime?)null : Saml20Utils.FromUtcString(value); }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the items.
        /// Items may be of types AudienceRestriction, Condition, OneTimeUse and ProxyRestriction
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("AudienceRestriction", typeof(AudienceRestriction), Order = 1)]
        [XmlElement("Condition", typeof(ConditionAbstract), Order = 1)]
        [XmlElement("OneTimeUse", typeof(OneTimeUse), Order = 1)]
        [XmlElement("ProxyRestriction", typeof(ProxyRestriction), Order = 1)]
        public List<ConditionAbstract> Items { get; set; }

        #endregion
    }
}
