using SAML2.AspNet.Config;
using SAML2.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace SAML2.AspNet
{
    public class WebConfigConfigurationProvider : IConfigurationProvider
    {
        public Saml2Configuration GetConfiguration()
        {
            var _config = ConfigurationManager.GetSection(Saml2Section.Name) as Saml2Section;

            if (_config == null) {
                throw new ConfigurationErrorsException(string.Format("Configuration section \"{0}\" not found", typeof(Saml2Section).Name));
            }

            _config.IdentityProviders.Refresh();
            return new Saml2Configuration
            {
                AllowedAudienceUris = _config.AllowedAudienceUris.Select(u => ToAllowedAudienceUri(u)).ToList(),
                AssertionProfileValidator = _config.AssertionProfile.AssertionValidator,
                CommonDomainCookie = ToCommonDomainCookie(_config.CommonDomainCookie),
                IdentityProviders = ToIdentityProviders(_config.IdentityProviders.Select(i => ToIdentityProvider(i)), _config.IdentityProviders),
                LoggingFactoryType = _config.Logging.LoggingFactory,
                Metadata = ToMetadata(_config.Metadata),
                ServiceProvider = ToServiceProvider(_config.ServiceProvider)
            };
        }

        private IdentityProviders ToIdentityProviders(IEnumerable<IdentityProvider> providers, IdentityProviderCollection config)
        {
            return new IdentityProviders(providers)
            {
                Encodings = config.Encodings,
                MetadataLocation = config.MetadataLocation,
                SelectionUrl = config.SelectionUrl
            };
        }

        private ServiceProvider ToServiceProvider(ServiceProviderElement serviceProvider)
        {
            if (serviceProvider == null) return null;
            return new ServiceProvider
            {
                AuthenticationContexts = new AuthenticationContexts(serviceProvider.AuthenticationContexts.Select(c => ToAuthenticationContext(c))) { Comparison = serviceProvider.AuthenticationContexts.Comparison },
                Endpoints = new ServiceProviderEndpoints(serviceProvider.Endpoints.Select(e => ToServiceProviderEndpoint(e))),
                Id = serviceProvider.Id,
                NameIdFormats = new NameIdFormats(serviceProvider.NameIdFormats.Select(f => ToNameIdFormat(f))),
                Server = serviceProvider.Server,
                SigningCertificate = serviceProvider.SigningCertificate.GetCertificate()
            };
        }

        private NameIdFormat ToNameIdFormat(NameIdFormatElement value)
        {
            return new NameIdFormat
            {
                Format = value.Format
            };
        }

        private ServiceProviderEndpoint ToServiceProviderEndpoint(ServiceProviderEndpointElement value)
        {
            return new ServiceProviderEndpoint
            {
                Binding = value.Binding,
                Index = value.Index,
                LocalPath = value.LocalPath,
                RedirectUrl = value.RedirectUrl,
                Type = value.Type
            };
        }

        private AuthenticationContext ToAuthenticationContext(AuthenticationContextElement value)
        {
            return new AuthenticationContext
            {
                Context = value.Context,
                ReferenceType = value.ReferenceType
            };
        }

        private Metadata ToMetadata(MetadataElement metadata)
        {
            return new Metadata
            {
                Contacts = metadata.Contacts.Cast<ContactElement>().Select(c => ToContact(c)).ToList(),
                ExcludeArtifactEndpoints = metadata.ExcludeArtifactEndpoints,
                Organization = ToOrganization(metadata.Organization),
                RequestedAttributes = metadata.RequestedAttributes.Select(a => ToAttribute(a)).ToList()
            };
        }

        private SAML2.Config.Attribute ToAttribute(AttributeElement value)
        {
            if (value == null) return null;
            return new SAML2.Config.Attribute
            {
                IsRequired = value.IsRequired,
                Name = value.Name
            };
        }

        private Organization ToOrganization(OrganizationElement organization)
        {
            if (organization == null) return null;
            return new Organization
            {
                DisplayName = organization.DisplayName,
                Name = organization.Name,
                Url = organization.Url
            };
        }

        private Contact ToContact(ContactElement contact)
        {
            return new Contact
            {
                Company = contact.Company,
                Email = contact.Email,
                GivenName = contact.GivenName,
                Phone = contact.Phone,
                SurName = contact.SurName,
                Type = contact.Type
            };
        }

        private CommonDomainCookie ToCommonDomainCookie(CommonDomainCookieElement commonDomainCookie)
        {
            return new CommonDomainCookie
            {
                Enabled = commonDomainCookie.Enabled,
                LocalReaderEndpoint = commonDomainCookie.LocalReaderEndpoint
            };
        }

        private Uri ToAllowedAudienceUri(AudienceUriElement uri)
        {
            return new Uri(uri.Uri);
        }

        private IdentityProvider ToIdentityProvider(IdentityProviderElement idp)
        {
            return new IdentityProvider
            {
                AllowIdPInitiatedSso = idp.AllowIdPInitiatedSso,
                AllowReplayAttacks = idp.AllowReplayAttacks,
                ArtifactResolution = ToArtifactResolution(idp.ArtifactResolution),
                AttributeQuery = ToAttributeQuery(idp.AttributeQuery),
                CertificateValidationTypes = idp.CertificateValidations.Select(v => v.Type).ToList(),
                CommonDomainCookie = idp.CommonDomainCookie.AllKeys.ToDictionary(k => k, k => idp.CommonDomainCookie[k].Value),
                Default = idp.Default,
                Endpoints = new IdentityProviderEndpoints(idp.Endpoints.Select(e => ToIdentityProviderEndpoint(e))),
                ForceAuth = idp.ForceAuth,
                Id = idp.Id,
                IsPassive = idp.IsPassive,
                Metadata = idp.Metadata,
                Name = idp.Name,
                OmitAssertionSignatureCheck = idp.OmitAssertionSignatureCheck,
                PersistentPseudonym = ToPersistentPseudonym(idp.PersistentPseudonym),
                QuirksMode = idp.QuirksMode,
                ResponseEncoding = idp.ResponseEncoding
            };
        }

        private HttpAuth ToAttributeQuery(HttpAuthElement attributeQuery)
        {
            if (attributeQuery == null) return null;
            return new HttpAuth
            {
                ClientCertificate = attributeQuery.ClientCertificate.GetCertificate(),
                Credentials = ToHttpAuthCredentials(attributeQuery.Credentials)
            };
        }

        private PersistentPseudonym ToPersistentPseudonym(PersistentPseudonymElement persistentPseudonym)
        {
            if (persistentPseudonym == null) return null;
            return new PersistentPseudonym
            {
                Mapper = persistentPseudonym.Mapper
            };
        }

        private HttpAuth ToArtifactResolution(HttpAuthElement artifactResolution)
        {
            if (artifactResolution == null) return null; 
            return new HttpAuth
            {
                ClientCertificate = ToX509Certificate2(artifactResolution.ClientCertificate),
                Credentials = ToHttpAuthCredentials(artifactResolution.Credentials)
            };
        }

        private X509Certificate2 ToX509Certificate2(CertificateElement clientCertificate)
        {
            return clientCertificate.GetCertificate();
        }

        private HttpAuthCredentials ToHttpAuthCredentials(HttpAuthCredentialsElement credentials)
        {
            if (credentials == null) return null;
            return new HttpAuthCredentials
            {
                Password = credentials.Password,
                Username = credentials.Username
            };
        }

        private IdentityProviderEndpoint ToIdentityProviderEndpoint(IdentityProviderEndpointElement value)
        {
            if (value == null) return null;
            return new IdentityProviderEndpoint
            {
                Binding = value.Binding,
                ForceProtocolBinding = value.ForceProtocolBinding,
                TokenAccessor = value.TokenAccessor,
                Type = value.Type,
                Url = value.Url
            };
        }
    }
}
