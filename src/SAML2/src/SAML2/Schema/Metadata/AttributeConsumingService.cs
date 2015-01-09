using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;AttributeConsumingService&gt; element defines a particular service offered by the service
    /// provider in terms of the attributes the service requires or desires.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class AttributeConsumingService
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "AttributeConsumingService";

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// Identifies the default service supported by the service provider. Useful if the specific service is not
        /// otherwise indicated by application context. If omitted, the value is assumed to be false.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool? IsDefault { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the index.
        /// A required attribute that assigns a unique integer value to the element so that it can be referenced
        /// in a protocol message.
        /// </summary>
        /// <value>The index.</value>
        [XmlAttribute("index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// Identifies the default service supported by the service provider. Useful if the specific service is not
        /// otherwise indicated by application context. If omitted, the value is assumed to be false.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("isDefault")]
        public string IsDefaultString
        {
            get { return IsDefault == null ? null : XmlConvert.ToString(IsDefault.Value); }
            set { IsDefault = string.IsNullOrEmpty(value) ? (bool?)null : XmlConvert.ToBoolean(value); }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the requested attribute.
        /// One or more elements specifying attributes required or desired by this service.
        /// </summary>
        /// <value>The requested attribute.</value>
        [XmlElement("RequestedAttribute", Order = 3)]
        public RequestedAttribute[] RequestedAttribute { get; set; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// One or more language-qualified names for the service.
        /// </summary>
        /// <value>The name of the service.</value>
        [XmlElement("ServiceName", Order = 1)]
        public LocalizedName[] ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the service description.
        /// Zero or more language-qualified strings that describe the service.
        /// </summary>
        /// <value>The service description.</value>
        [XmlElement("ServiceDescription", Order = 2)]
        public LocalizedName[] ServiceDescription { get; set; }

        #endregion
    }
}
