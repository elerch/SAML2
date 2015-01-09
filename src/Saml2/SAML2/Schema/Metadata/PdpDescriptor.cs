using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The <c>&lt;PDPDescriptor&gt;</c> element extends RoleDescriptorType with content reflecting profiles specific to
    /// policy decision points, SAML authorities that respond to <c>&lt;samlp:AuthzDecisionQuery&gt;</c> messages.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class PdpDescriptor : RoleDescriptor
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "Organization";

        #region Elements

        /// <summary>
        /// Gets or sets the authorize service.
        /// One or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Authorization Decision Query protocol defined in [SAMLProf]. All policy decision points support
        /// at least one such endpoint, by definition.
        /// </summary>
        /// <value>The authorize service.</value>
        [XmlElement("AuthzService", Order = 1)]
        public Endpoint[] AuthzService { get; set; }

        /// <summary>
        /// Gets or sets the assertion ID request service.
        /// Zero or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Assertion Request protocol defined in [SAMLProf] or the special URI binding for assertion
        /// requests defined in [SAMLBind].
        /// </summary>
        /// <value>The assertion ID request service.</value>
        [XmlElement("AssertionIDRequestService", Order = 2)]
        public Endpoint[] AssertionIdRequestService { get; set; }

        /// <summary>
        /// Gets or sets the name ID format.
        /// Zero or more elements of type anyURI that enumerate the name identifier formats supported by
        /// this authority.
        /// </summary>
        /// <value>The name ID format.</value>
        [XmlElement("NameIDFormat", DataType = "anyURI", Order = 3)]
        public string[] NameIdFormat { get; set; }

        #endregion
    }
}
