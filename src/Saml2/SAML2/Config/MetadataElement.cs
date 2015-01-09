using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Metadata configuration element.
    /// </summary>
    public class MetadataElement : WritableConfigurationElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets a value indicating whether to exclude artifact endpoints in metadata generation.
        /// </summary>
        /// <value><c>true</c> if exclude artifact endpoints; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("excludeArtifactEndpoints")]
        public bool ExcludeArtifactEndpoints
        {
            get { return (bool)base["excludeArtifactEndpoints"]; }
            set { base["excludeArtifactEndpoints"] = value; }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>The contacts.</value>
        [ConfigurationProperty("contacts")]
        public ContactCollection Contacts
        {
            get { return (ContactCollection)base["contacts"]; }
            set { base["contacts"] = value; }
        }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>The organization.</value>
        [ConfigurationProperty("organization")]
        public OrganizationElement Organization
        {
            get { return (OrganizationElement)base["organization"]; }
            set { base["organization"] = value; }
        }

        /// <summary>
        /// Gets or sets the requested attributes.
        /// </summary>
        /// <value>The requested attributes.</value>
        [ConfigurationProperty("requestedAttributes")]
        public RequestedAttributesCollection RequestedAttributes
        {
            get { return (RequestedAttributesCollection)base["requestedAttributes"]; }
            set { base["requestedAttributes"] = value; }
        }

        #endregion
    }
}
