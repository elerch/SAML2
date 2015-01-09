using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The <c>&lt;AttributeAuthorityDescriptor&gt;</c> element extends RoleDescriptorType with content
    /// reflecting profiles specific to attribute authorities, SAML authorities that respond to
    /// <c>&lt;samlp:AttributeQuery&gt;</c> messages.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Metadata)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Metadata, IsNullable = false)]
    public class AttributeAuthorityDescriptor : RoleDescriptor
    {   
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public new const string ElementName = "AttributeAuthorityDescriptor";

        #region Elements

        /// <summary>
        /// Gets or sets the attribute.
        /// Zero or more elements that identify the SAML attributes supported by the authority. Specific
        /// values MAY optionally be included, indicating that only certain values permitted by the attribute's
        /// definition are supported.
        /// </summary>
        /// <value>The attribute.</value>
        [XmlElement("Attribute", Namespace = Saml20Constants.Metadata, Order = 5)]
        public Attribute[] Attribute { get; set; }

        /// <summary>
        /// Gets or sets the attribute profile.
        /// Zero or more elements of type anyURI that enumerate the attribute profiles supported by this
        /// authority.
        /// </summary>
        /// <value>The attribute profile.</value>
        [XmlElement("AttributeProfile", DataType = "anyURI", Order = 4)]
        public string[] AttributeProfile { get; set; }

        /// <summary>
        /// Gets or sets the attribute service.
        /// One or more elements of type EndpointType that describe endpoints that support the profile of
        /// the Attribute Query protocol defined in [SAMLProf]. All attribute authorities support at least one
        /// such endpoint, by definition.
        /// </summary>
        /// <value>The attribute service.</value>
        [XmlElement("AttributeService", Order = 1)]
        public Endpoint[] AttributeService { get; set; }

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
