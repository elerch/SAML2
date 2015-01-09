using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The <c>&lt;AuthnAuthorityDescriptor&gt;</c> element extends RoleDescriptorType with content reflecting
    /// profiles specific to authentication authorities, SAML authorities that respond to <c>&lt;samlp:AuthnQuery&gt;</c>
    /// messages.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class AuthnAuthorityDescriptor : RoleDescriptor 
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AuthnAuthorityDescriptor";

        #region Elements

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
        /// Gets or sets the authentication query service.
        /// One or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Authentication Query protocol defined in [SAMLProf]. All authentication authorities support at
        /// least one such endpoint, by definition.
        /// </summary>
        /// <value>The authentication query service.</value>
        [XmlElement("AuthnQueryService", Order = 1)]
        public Endpoint[] AuthnQueryService { get; set; }

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
