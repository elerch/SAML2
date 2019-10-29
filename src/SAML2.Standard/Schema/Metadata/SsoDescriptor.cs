using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The SSODescriptorType abstract type is a common base type for the concrete types
    /// SPSSODescriptorType and IDPSSODescriptorType, described in subsequent sections. It extends
    /// RoleDescriptorType with elements reflecting profiles common to both identity providers and service
    /// providers that support SSO
    /// </summary>
    [XmlInclude(typeof(SpSsoDescriptor))]
    [XmlIncludeAttribute(typeof(IdpSsoDescriptor))]
    [Serializable]
    [DebuggerStepThrough]
    [XmlTypeAttribute(Namespace = Saml20Constants.Metadata)]
    public abstract class SsoDescriptor : RoleDescriptor
    {
        #region Elements

        /// <summary>
        /// Gets or sets the artifact resolution service.
        /// Zero or more elements of type IndexedEndpointType that describe indexed endpoints that
        /// support the Artifact Resolution profile defined in [SAMLProf]. The ResponseLocation attribute
        /// MUST be omitted.
        /// </summary>
        /// <value>The artifact resolution service.</value>
        [XmlElement("ArtifactResolutionService", Order = 1)]
        public IndexedEndpoint[] ArtifactResolutionService { get; set; }

        /// <summary>
        /// Gets or sets the manage name ID service.
        /// Zero or more elements of type EndpointType that describe endpoints that support the Name
        /// Identifier Management profiles defined in [SAMLProf].
        /// </summary>
        /// <value>The manage name ID service.</value>
        [XmlElement("ManageNameIDService", Order = 3)]
        public Endpoint[] ManageNameIdService { get; set; }

        /// <summary>
        /// Gets or sets the name ID format.
        /// Zero or more elements of type anyURI that enumerate the name identifier formats supported by
        /// this system entity acting in this role. See Section 8.3 of [SAMLCore] for some possible values for
        /// this element.
        /// </summary>
        /// <value>The name ID format.</value>
        [XmlElement("NameIDFormat", DataType = "anyURI", Order = 4)]
        public string[] NameIdFormat { get; set; }

        /// <summary>
        /// Gets or sets the single logout service.
        /// Zero or more elements of type EndpointType that describe endpoints that support the Single
        /// Logout profiles defined in [SAMLProf].
        /// </summary>
        /// <value>The single logout service.</value>
        [XmlElement("SingleLogoutService", Order = 2)]
        public Endpoint[] SingleLogoutService { get; set; }

        #endregion
    }
}
