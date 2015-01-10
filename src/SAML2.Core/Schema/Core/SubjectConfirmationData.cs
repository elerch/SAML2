using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Utils;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;SubjectConfirmationData&gt; element has the SubjectConfirmationDataType complex type. It
    /// specifies additional data that allows the subject to be confirmed or constrains the circumstances under
    /// which the act of subject confirmation can take place. Subject confirmation takes place when a relying
    /// party seeks to verify the relationship between an entity presenting the assertion (that is, the attesting
    /// entity) and the subject of the assertion's claims.
    /// </summary>
    [XmlInclude(typeof(KeyInfoConfirmationData))]
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class SubjectConfirmationData
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "SubjectConfirmationData";

        /// <summary>
        /// Gets or sets the not before.
        /// </summary>
        /// <value>The not before.</value>
        [XmlIgnore]
        public DateTime? NotBefore { get; set; }

        /// <summary>
        /// Gets or sets the not on or after.
        /// </summary>
        /// <value>The not on or after.</value>
        [XmlIgnore]
        public DateTime? NotOnOrAfter { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        [XmlAttribute("Address", DataType = "string")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets XML any attribute.
        /// </summary>
        /// <value>The XML Any attribute.</value>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        /// <summary>
        /// Gets or sets the in response to.
        /// </summary>
        /// <value>The in response to.</value>
        [XmlAttribute("InResponseTo", DataType = "NCName")]
        public string InResponseTo { get; set; }

        /// <summary>
        /// Gets or sets the not on or after string.
        /// </summary>
        /// <value>The not on or after string.</value>
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

        /// <summary>
        /// Gets or sets the recipient.
        /// </summary>
        /// <value>The recipient.</value>
        [XmlAttribute("Recipient", DataType = "anyURI")]
        public string Recipient { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the any-elements-array.
        /// </summary>
        /// <value>The any-elements-array</value>
        [XmlAnyElement(Order = 1)]
        public XmlElement[] AnyElements { get; set; }

        #endregion
    }
}
