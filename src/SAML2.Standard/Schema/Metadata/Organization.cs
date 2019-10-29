using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;Organization&gt; element specifies basic information about an organization responsible for a SAML
    /// entity or role. The use of this element is always optional. Its content is informative in nature and does not
    /// directly map to any core SAML elements or attributes.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName, IsNullable = false)]
    public class Organization
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Organization";

        #region Attributes

        /// <summary>
        /// Gets or sets XML any attribute.
        /// </summary>
        /// <value>The XML Any attribute.</value>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the extensions.
        /// This contains optional metadata extensions that are agreed upon between a metadata publisher
        /// and consumer. Extensions MUST NOT include global (non-namespace-qualified) elements or
        /// elements qualified by a SAML-defined namespace within this element.
        /// </summary>
        /// <value>The extensions.</value>
        [XmlElement("Extensions", Order = 1)]
        public ExtensionType Extensions { get; set; }

        /// <summary>
        /// Gets or sets the display name of the organization.
        /// One or more language-qualified names that are suitable for human consumption.
        /// </summary>
        /// <value>The display name of the organization.</value>
        [XmlElement("OrganizationDisplayName", Order = 3)]
        public LocalizedName[] OrganizationDisplayName { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the organization.
        /// One or more language-qualified names that may or may not be suitable for human consumption
        /// </summary>
        /// <value>The name of the organization.</value>
        [XmlElement("OrganizationName", Order = 2)]
        public LocalizedName[] OrganizationName { get; set; }
        
        /// <summary>
        /// Gets or sets the organization URL.
        /// One or more language-qualified URIs that specify a location to which to direct a user for additional
        /// information. Note that the language qualifier refers to the content of the material at the specified
        /// location.
        /// </summary>
        /// <value>The organization URL.</value>
        [XmlElement("OrganizationURL", Order = 4)]
        public LocalizedURI[] OrganizationURL { get; set; }

        #endregion
    }
}
