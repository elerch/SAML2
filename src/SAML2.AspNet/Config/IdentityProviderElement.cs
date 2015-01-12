using System.Configuration;

namespace SAML2.AspNet.Config
{
    /// <summary>
    /// Identity Provider configuration element.
    /// </summary>
    public class IdentityProviderElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public Saml20MetadataDocument Metadata { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IdentityProviderElement"/> is default.
        /// </summary>
        /// <value><c>true</c> if default; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Use default in case common domain cookie is not set, and more than one endpoint is available.
        /// </remarks>
        [ConfigurationProperty("default")]
        public bool Default
        {
            get { return (bool)base["default"]; }
            set { base["default"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force authentication on each <c>AuthnRequest</c>.
        /// </summary>
        /// <value><c>true</c> if force authentication; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("forceAuth")]
        public bool ForceAuth
        {
            get { return (bool)base["forceAuth"]; }
            set { base["forceAuth"] = value; }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [ConfigurationProperty("id", IsKey = true, IsRequired = true)]
        public string Id
        {
            get { return (string)base["id"]; }
            set { base["id"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <c>AuthnRequest</c> should be passive.
        /// </summary>
        /// <value><c>true</c> if this instance is passive; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("isPassive")]
        public bool IsPassive
        {
            get { return (bool)base["isPassive"]; }
            set { base["isPassive"] = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to omit assertion signature check.
        /// </summary>
        /// <value><c>true</c> if assertion signature check should be omitted; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("omitAssertionSignatureCheck")]
        public bool OmitAssertionSignatureCheck
        {
            get { return (bool)base["omitAssertionSignatureCheck"]; }
            set { base["omitAssertionSignatureCheck"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow IdP Initiated SSO. This profile allows SAML responses without an SP-initiated request.
        /// </summary>
        /// <value><c>true</c> if IdP Initiated SSO should be allowed; otherwise, <c>false</c></value>>
        [ConfigurationProperty("allowIdPInitiatedSso")]
        public bool AllowIdPInitiatedSso 
        {
            get { return (bool)base["allowIdPInitiatedSso"]; }
            set { base["allowIdPInitiatedSso"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow replay attacks. This is more aggressive than the Idp Initiated SSO in that the InReponseTo attribute will be completely ignored. Do not use this unless your IdP is inadvertently sending InResponseTo values when it should not.
        /// </summary>
        /// <value><c>true</c> if replay attacks should be allowed; otherwise, <c>false</c></value>>
        [ConfigurationProperty("allowReplayAttacks")]
        public bool AllowReplayAttacks
        {
            get { return (bool)base["allowReplayAttacks"]; }
            set { base["allowReplayAttacks"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quirks mode should be enabled.
        /// </summary>
        /// <value><c>true</c> if quirks mode should be enabled; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("quirksMode")]
        public bool QuirksMode
        {
            get { return (bool)base["quirksMode"]; }
            set { base["quirksMode"] = value; }
        }

        /// <summary>
        /// Gets or sets a value for overriding option for the default UTF-8 encoding convention on SAML responses
        /// </summary>
        /// <value>The response encoding.</value>
        [ConfigurationProperty("responseEncoding")]
        public string ResponseEncoding
        {
            get { return (string)base["responseEncoding"]; }
            set { base["responseEncoding"] = value; }
        }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the artifact resolution.
        /// </summary>
        /// <value>The artifact resolution.</value>
        [ConfigurationProperty("artifactResolution")]
        public HttpAuthElement ArtifactResolution
        {
            get { return (HttpAuthElement)base["artifactResolution"]; }
            set { base["artifactResolution"] = value; }
        }

        /// <summary>
        /// Gets or sets the attribute query configuration parameters.
        /// </summary>
        /// <value>The attribute query.</value>
        [ConfigurationProperty("attributeQuery")]
        public HttpAuthElement AttributeQuery
        {
            get { return (HttpAuthElement)base["attributeQuery"]; }
            set { base["attributeQuery"] = value; }
        }

        /// <summary>
        /// Gets or sets the certificate validations.
        /// </summary>
        /// <value>The certificate validations.</value>
        [ConfigurationProperty("certificateValidations")]
        public CertificateValidationCollection CertificateValidations
        {
            get { return (CertificateValidationCollection)base["certificateValidations"]; }
            set { base["certificateValidations"] = value; }
        }

        /// <summary>
        /// Gets or sets the common domain cookie configuration settings.
        /// </summary>
        /// <value>The common domain cookie.</value>
        [ConfigurationProperty("commonDomainCookie")]
        public KeyValueConfigurationCollection CommonDomainCookie
        {
            get { return (KeyValueConfigurationCollection)base["commonDomainCookie"]; }
            set { base["commonDomainCookie"] = value; }
        }

        /// <summary>
        /// Gets or sets the endpoints.
        /// </summary>
        /// <value>The endpoints.</value>
        [ConfigurationProperty("endpoints")]
        public IdentityProviderEndpointCollection Endpoints
        {
            get { return (IdentityProviderEndpointCollection)base["endpoints"]; }
            set { base["endpoints"] = value; }
        }

        /// <summary>
        /// Gets or sets the persistent pseudonym configuration settings.
        /// </summary>
        /// <value>The persistent pseudonym.</value>
        [ConfigurationProperty("persistentPseudonym")]
        public PersistentPseudonymElement PersistentPseudonym
        {
            get { return (PersistentPseudonymElement)base["persistentPseudonym"]; }
            set { base["persistentPseudonym"] = value; }
        }

        #endregion

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Id; }
        }

        #endregion
    }
}
