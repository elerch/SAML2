using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The &lt;IDPEntry&gt; element specifies a single identity provider trusted by the requester to authenticate the
    /// presenter.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class IdpEntry
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "IDPEntry";

        #region Attributes

        /// <summary>
        /// Gets or sets the provider ID.
        /// The unique identifier of the identity provider.
        /// </summary>
        /// <value>The provider ID.</value>
        [XmlAttribute("ProviderID", DataType = "anyURI")]
        public string ProviderID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// A human-readable name for the identity provider
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// A URI reference representing the location of a profile-specific endpoint supporting the authentication
        /// request protocol. The binding to be used must be understood from the profile of use.
        /// </summary>
        /// <value>The location.</value>
        [XmlAttribute("Loc", DataType = "anyURI")]
        public string Location { get; set; }

        #endregion
    }
}
