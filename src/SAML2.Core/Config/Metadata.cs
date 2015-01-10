using SAML2.Schema.Metadata;
using System.Collections.Generic;
using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Metadata configuration element.
    /// </summary>
    public class Metadata 
    {
        

        /// <summary>
        /// Gets or sets a value indicating whether to exclude artifact endpoints in metadata generation.
        /// </summary>
        /// <value><c>true</c> if exclude artifact endpoints; otherwise, <c>false</c>.</value>
        
        public bool ExcludeArtifactEndpoints { get; set; }

        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>The contacts.</value>

        public List<Contact> Contacts { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>The organization.</value>
        
        public Organization Organization { get; set; }

        /// <summary>
        /// Gets or sets the requested attributes.
        /// </summary>
        /// <value>The requested attributes.</value>
        public List<RequestedAttribute> RequestedAttributes { get; set; }
    }
}
