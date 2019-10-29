using System;
using System.Xml;
using System.Xml.Serialization;
using SAML2.Schema.Core;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;RequestedAttribute&gt; element specifies a service provider's interest in a specific SAML
    /// attribute, optionally including specific values.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class RequestedAttribute : SamlAttribute
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "RequestedAttribute";

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// Optional XML attribute indicates if the service requires the corresponding SAML attribute in order
        /// to function at all (as opposed to merely finding an attribute useful or desirable).
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool? IsRequired { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the is required string.
        /// </summary>
        /// <value>The is required string.</value>
        [XmlAttribute("isRequired")]
        public string IsRequiredString
        {
            get { return IsRequired.HasValue ? XmlConvert.ToString(IsRequired.Value) : null; }
            set { IsRequired = string.IsNullOrEmpty(value) ? (bool?)null : XmlConvert.ToBoolean(value); }
        }

        #endregion
    }
}
