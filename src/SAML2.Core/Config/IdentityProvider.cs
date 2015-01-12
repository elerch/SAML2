using System.Collections.Generic;
using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Identity Provider configuration element.
    /// </summary>
    public class IdentityProvider
    {
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public Saml20MetadataDocument Metadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IdentityProvider"/> is default.
        /// </summary>
        /// <value><c>true</c> if default; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Use default in case common domain cookie is not set, and more than one endpoint is available.
        /// </remarks>
        public bool Default{ get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to force authentication on each <c>AuthnRequest</c>.
        /// </summary>
        /// <value><c>true</c> if force authentication; otherwise, <c>false</c>.</value>
        public bool ForceAuth { get; set; }
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <c>AuthnRequest</c> should be passive.
        /// </summary>
        /// <value><c>true</c> if this instance is passive; otherwise, <c>false</c>.</value>
        public bool IsPassive { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to omit assertion signature check.
        /// </summary>
        /// <value><c>true</c> if assertion signature check should be omitted; otherwise, <c>false</c>.</value>
        public bool OmitAssertionSignatureCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow IdP Initiated SSO. This profile allows SAML responses without an SP-initiated request.
        /// </summary>
        /// <value><c>true</c> if IdP Initiated SSO should be allowed; otherwise, <c>false</c></value>>
        public bool AllowIdPInitiatedSso { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to allow replay attacks. This is more aggressive than the Idp Initiated SSO in that the InReponseTo attribute will be completely ignored. Do not use this unless your IdP is inadvertently sending InResponseTo values when it should not.
        /// </summary>
        /// <value><c>true</c> if replay attacks should be allowed; otherwise, <c>false</c></value>>
        public bool AllowReplayAttacks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether quirks mode should be enabled.
        /// </summary>
        /// <value><c>true</c> if quirks mode should be enabled; otherwise, <c>false</c>.</value>
        public bool QuirksMode { get; set; }

        /// <summary>
        /// Gets or sets a value for overriding option for the default UTF-8 encoding convention on SAML responses
        /// </summary>
        /// <value>The response encoding.</value>
        public string ResponseEncoding { get; set; }


        /// <summary>
        /// Gets or sets the artifact resolution.
        /// </summary>
        /// <value>The artifact resolution.</value>
        public HttpAuth ArtifactResolution { get; set; }

        /// <summary>
        /// Gets or sets the attribute query configuration parameters.
        /// </summary>
        /// <value>The attribute query.</value>
        public HttpAuth AttributeQuery { get; set; }

        /// <summary>
        /// Gets or sets the certificate validations.
        /// </summary>
        /// <value>The certificate validations.</value>
        public List<string> CertificateValidationTypes { get; set; }


        /// <summary>
        /// Gets or sets the common domain cookie configuration settings.
        /// </summary>
        /// <value>The common domain cookie.</value>
        public IDictionary<string,string> CommonDomainCookie { get; set; }

        /// <summary>
        /// Gets or sets the endpoints.
        /// </summary>
        /// <value>The endpoints.</value>
        public IdentityProviderEndpoints Endpoints { get; set; }

        /// <summary>
        /// Gets or sets the persistent pseudonym configuration settings.
        /// </summary>
        /// <value>The persistent pseudonym.</value>
        public PersistentPseudonym PersistentPseudonym { get; set; }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Id; }
        }

    }
}
