using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Schema.Metadata;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// The <see cref="Saml20MetadataDocument"/> class handles functionality related to the &lt;EntityDescriptor&gt; element.
    /// If a received metadata document contains a &lt;EntitiesDescriptor&gt; element, it is necessary to use an
    /// instance of this class for each &lt;EntityDescriptor&gt; contained.
    /// </summary>
    public class Saml20MetadataDocument
    {
        #region Fields

        /// <summary>
        /// AssertionConsumerServiceEndpoints backing field.
        /// </summary>
        private List<IdentityProviderEndpointElement> _assertionConsumerServiceEndpoints;

        /// <summary>
        /// Attribute query endpoints.
        /// </summary>
        private List<Endpoint> _attributeQueryEndpoints;

        /// <summary>
        /// ARSEndpoints backing field.
        /// </summary>
        private Dictionary<int, IndexedEndpoint> _idpArsEndpoints;

        /// <summary>
        /// SLOEndpoints backing field.
        /// </summary>
        private List<IdentityProviderEndpointElement> _idpSloEndpoints;

        /// <summary>
        /// The keys.
        /// </summary>
        private List<KeyDescriptor> _keys;

        /// <summary>
        /// ARSEndpoints backing field.
        /// </summary>
        private Dictionary<int, IndexedEndpoint> _spArsEndpoints;

        /// <summary>
        /// SLOEndpoints backing field.
        /// </summary>
        private List<IdentityProviderEndpointElement> _spSloEndpoints;

        /// <summary>
        /// SSOEndpoints backing field.
        /// </summary>
        private List<IdentityProviderEndpointElement> _ssoEndpoints;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        public Saml20MetadataDocument() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor.</param>
        public Saml20MetadataDocument(XmlDocument entityDescriptor) 
            : this()
        {
            if (XmlSignatureUtils.IsSigned(entityDescriptor))
            {
                if (!XmlSignatureUtils.CheckSignature(entityDescriptor))
                {
                    throw new Saml20Exception("Metadata signature could not be verified.");
                }
            }

            ExtractKeyDescriptors(entityDescriptor);
            Entity = Serialization.DeserializeFromXmlString<EntityDescriptor>(entityDescriptor.OuterXml);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        /// <param name="sign">if set to <c>true</c> the metadata document will be signed.</param>
        public Saml20MetadataDocument(bool sign)
            : this()
        {
            Sign = sign;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20MetadataDocument"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="keyinfo">key information for the service provider certificate.</param>
        /// <param name="sign">if set to <c>true</c> the metadata document will be signed.</param>
        public Saml20MetadataDocument(Saml2Section config, KeyInfo keyinfo, bool sign)
            : this(sign)
        {
            ConvertToMetadata(config, keyinfo);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the endpoints specified in the <c>&lt;AssertionConsumerService&gt;</c> element in the <c>SpSsoDescriptor</c>.
        /// These endpoints are only applicable if we are reading metadata issued by a service provider.
        /// </summary>
        public List<IdentityProviderEndpointElement> AssertionConsumerServiceEndpoints
        {
            get
            {
                if (_assertionConsumerServiceEndpoints == null)
                {
                    ExtractEndpoints();
                }

                return _assertionConsumerServiceEndpoints;
            }
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public EntityDescriptor Entity { get; private set; }

        /// <summary>
        /// Gets the ID of the entity described in the document.
        /// </summary>
        public string EntityId
        {
            get
            {
                if (Entity != null)
                {
                    return Entity.EntityID;
                }

                throw new InvalidOperationException("This instance does not contain a metadata document");
            }
        }

        /// <summary>
        /// Gets the IDP SLO endpoints.
        /// </summary>
        public List<IdentityProviderEndpointElement> IDPSLOEndpoints
        {
            get
            {
                if (_idpSloEndpoints == null)
                {
                    ExtractEndpoints();
                }

                return _idpSloEndpoints;
            }
        }

        /// <summary>
        /// Gets the keys contained in the metadata document.
        /// </summary>
        public List<KeyDescriptor> Keys
        {
            get
            {
                if (_keys == null)
                {
                    ExtractKeyDescriptors();
                }

                return _keys;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the metadata should be signed when the ToXml() method is called.
        /// </summary>
        public bool Sign { get; set; }

        /// <summary>
        /// Gets the SP SLO endpoints.
        /// </summary>
        public List<IdentityProviderEndpointElement> SPSLOEndpoints
        {
            get
            {
                if (_spSloEndpoints == null)
                {
                    ExtractEndpoints();
                }

                return _spSloEndpoints;
            }
        }

        /// <summary>
        /// Gets the SSO endpoints.
        /// </summary>
        public List<IdentityProviderEndpointElement> SSOEndpoints
        {
            get
            {
                if (_ssoEndpoints == null)
                {
                    ExtractEndpoints();
                }

                return _ssoEndpoints;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a default entity in the 
        /// </summary>
        /// <returns>The default <see cref="EntityDescriptor"/>.</returns>
        public EntityDescriptor CreateDefaultEntity()
        {
            if (Entity != null)
            {
                throw new InvalidOperationException("An entity is already created in this document.");
            }

            Entity = GetDefaultEntityInstance();

            return Entity;
        }

        /// <summary>
        /// Gets the IDP ArtifactResolutionService endpoint.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The artifact resolution service endpoint.</returns>
        public string GetIDPARSEndpoint(ushort index)
        {
            var ep = _idpArsEndpoints[index];
            return ep != null ? ep.Location : string.Empty;
        }

        /// <summary>
        /// Gets all AttributeQuery endpoints.
        /// </summary>
        /// <returns>The List of attribute query endpoints.</returns>
        public List<Endpoint> GetAttributeQueryEndpoints()
        {
            if (_attributeQueryEndpoints == null)
            {
                ExtractEndpoints();
            }

            return _attributeQueryEndpoints;
        }

        /// <summary>
        /// Gets the location of the first AttributeQuery endpoint.
        /// </summary>
        /// <returns>The attribute query endpoint location.</returns>
        public string GetAttributeQueryEndpointLocation()
        {
            var endpoints = GetAttributeQueryEndpoints();
            if (endpoints.Count == 0)
            {
                throw new Saml20Exception("The identity provider does not support attribute queries.");
            }

            return endpoints[0].Location;
        }

        /// <summary>
        /// Retrieves the keys marked with the usage given as parameter.
        /// </summary>
        /// <param name="usage">The usage.</param>
        /// <returns>A list containing the keys. If no key is marked with the given usage, the method returns an empty list.</returns>
        public List<KeyDescriptor> GetKeys(KeyTypes usage)
        {
            return Keys.FindAll(desc => desc.Use == usage);
        }

        /// <summary>
        /// Return a string containing the metadata XML based on the settings added to this instance.
        /// The resulting XML will be signed, if the AsymmetricAlgorithm property has been set.
        /// The default encoding (UTF-8) will be used for the resulting XML.
        /// </summary>
        /// <returns>The XML.</returns>
        public string ToXml()
        {
            return ToXml(null);
        }

        /// <summary>
        /// Return a string containing the metadata XML based on the settings added to this instance.
        /// The resulting XML will be signed, if the AsymmetricAlgorithm property has been set.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The XML.</returns>
        public string ToXml(Encoding encoding)
        {
            return ToXml(encoding, null);
        }
        /// <summary>
        /// Return a string containing the metadata XML based on the settings added to this instance.
        /// The resulting XML will be signed, if the AsymmetricAlgorithm property has been set.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>The XML.</returns>
        public string ToXml(Encoding encoding, Saml2Section config)
        {
            encoding = encoding ?? Encoding.UTF8;
            config = config ?? Saml2Config.GetConfig();
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(Serialization.SerializeToXmlString(Entity));

            // Add the correct encoding to the head element.
            if (doc.FirstChild is XmlDeclaration)
            {
                ((XmlDeclaration)doc.FirstChild).Encoding = encoding.WebName;
            }
            else
            {
                doc.PrependChild(doc.CreateXmlDeclaration("1.0", encoding.WebName, null));
            }

            if (Sign)
            {
                SignDocument(doc, config);
            }

            return doc.OuterXml;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the binding.
        /// </summary>
        /// <param name="samlBinding">The SAML binding.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The binding.</returns>
        private static string GetBinding(BindingType samlBinding, string defaultValue)
        {
            switch (samlBinding)
            {
                case BindingType.Artifact:
                    return Saml20Constants.ProtocolBindings.HttpArtifact;
                case BindingType.Post:
                    return Saml20Constants.ProtocolBindings.HttpPost;
                case BindingType.Redirect:
                    return Saml20Constants.ProtocolBindings.HttpRedirect;
                case BindingType.Soap:
                    return Saml20Constants.ProtocolBindings.HttpSoap;
                case BindingType.NotSet:
                    return defaultValue;
                default:
                    throw new ConfigurationErrorsException(string.Format("Unsupported SAML binding {0}", Enum.GetName(typeof(BindingType), samlBinding)));
            }
        }

        /// <summary>
        /// Gets the default entity instance.
        /// </summary>
        /// <returns>The default <see cref="EntityDescriptor"/>.</returns>
        private static EntityDescriptor GetDefaultEntityInstance()
        {
            var result = new EntityDescriptor
            {
                ID = "id" + Guid.NewGuid().ToString("N")
            };

            return result;
        }

        /// <summary>
        /// Signs the document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private static void SignDocument(XmlDocument doc, Saml2Section config)
        {
            var cert = config.ServiceProvider.SigningCertificate.GetCertificate();
            if (!cert.HasPrivateKey)
            {
                throw new InvalidOperationException("Private key access to the signing certificate is required.");
            }

            var signedXml = new SignedXml(doc);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SigningKey = cert.PrivateKey;

            // Retrieve the value of the "ID" attribute on the root assertion element.
            var reference = new Reference("#" + doc.DocumentElement.GetAttribute("ID"));

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());

            signedXml.AddReference(reference);

            // Include the public key of the certificate in the assertion.
            signedXml.KeyInfo = new KeyInfo();
            // This was WholeChain, requiring us to trust our certs. WIF WSFed does not have this requirement,
            // so the option was relaxed to EndCertOnly. This might need to be configurable
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.EndCertOnly)); 

            signedXml.ComputeSignature();

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            doc.DocumentElement.InsertBefore(doc.ImportNode(signedXml.GetXml(), true), doc.DocumentElement.FirstChild);
        }

        /// <summary>
        /// Takes the configuration class and converts it to a SAML2.0 metadata document.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="keyInfo">The keyInfo.</param>
        private void ConvertToMetadata(Saml2Section config, KeyInfo keyInfo)
        {
            var entity = CreateDefaultEntity();
            entity.EntityID = config.ServiceProvider.Id;
            entity.ValidUntil = DateTime.Now.AddDays(7);

            var serviceProviderDescriptor = new SpSsoDescriptor
                                   {
                                       ProtocolSupportEnumeration = new[] { Saml20Constants.Protocol },
                                       AuthnRequestsSigned = XmlConvert.ToString(true),
                                       WantAssertionsSigned = XmlConvert.ToString(true)
                                   };

            if (config.ServiceProvider.NameIdFormats.Count > 0)
            {
                serviceProviderDescriptor.NameIdFormat = new string[config.ServiceProvider.NameIdFormats.Count];
                var count = 0;
                foreach (var elem in config.ServiceProvider.NameIdFormats)
                {
                    serviceProviderDescriptor.NameIdFormat[count++] = elem.Format;
                }
            }
            
            var baseUrl = new Uri(config.ServiceProvider.Server);
            var logoutServiceEndpoints = new List<Endpoint>();
            var signonServiceEndpoints = new List<IndexedEndpoint>();

            var artifactResolutionEndpoints = new List<IndexedEndpoint>(2);

            // Include endpoints.
            foreach (var endpoint in config.ServiceProvider.Endpoints)
            {
                if (endpoint.Type == EndpointType.SignOn)
                {
                    var loginEndpoint = new IndexedEndpoint
                                            {
                                                Index = endpoint.Index,
                                                IsDefault = true,
                                                Location = new Uri(baseUrl, endpoint.LocalPath).ToString(),
                                                Binding = GetBinding(endpoint.Binding, Saml20Constants.ProtocolBindings.HttpPost)
                                            };
                    signonServiceEndpoints.Add(loginEndpoint);

                    var artifactSignonEndpoint = new IndexedEndpoint
                                                     {
                                                         Binding = Saml20Constants.ProtocolBindings.HttpSoap,
                                                         Index = loginEndpoint.Index,
                                                         Location = loginEndpoint.Location
                                                     };
                    artifactResolutionEndpoints.Add(artifactSignonEndpoint);

                    continue;
                }

                if (endpoint.Type == EndpointType.Logout)
                {
                    var logoutEndpoint = new Endpoint
                                             {
                                                 Location = new Uri(baseUrl, endpoint.LocalPath).ToString()
                                             };
                    logoutEndpoint.ResponseLocation = logoutEndpoint.Location;
                    logoutEndpoint.Binding = GetBinding(endpoint.Binding, Saml20Constants.ProtocolBindings.HttpPost);
                    logoutServiceEndpoints.Add(logoutEndpoint);

                    // TODO: Look at this...
                    logoutEndpoint = new Endpoint
                                         {
                                             Location = new Uri(baseUrl, endpoint.LocalPath).ToString()
                                         };
                    logoutEndpoint.ResponseLocation = logoutEndpoint.Location;
                    logoutEndpoint.Binding = GetBinding(endpoint.Binding, Saml20Constants.ProtocolBindings.HttpRedirect);
                    logoutServiceEndpoints.Add(logoutEndpoint);

                    var artifactLogoutEndpoint = new IndexedEndpoint
                                                     {
                                                         Binding = Saml20Constants.ProtocolBindings.HttpSoap,
                                                         Index = endpoint.Index,
                                                         Location = logoutEndpoint.Location
                                                     };
                    artifactResolutionEndpoints.Add(artifactLogoutEndpoint);
                    
                    continue;
                }
            }           

            serviceProviderDescriptor.SingleLogoutService = logoutServiceEndpoints.ToArray();
            serviceProviderDescriptor.AssertionConsumerService = signonServiceEndpoints.ToArray();
            
            // Attribute consuming service. 
            if (config.Metadata.RequestedAttributes.Count > 0)
            {
                var attConsumingService = new AttributeConsumingService();
                serviceProviderDescriptor.AttributeConsumingService = new[] { attConsumingService };
                attConsumingService.Index = signonServiceEndpoints[0].Index;
                attConsumingService.IsDefault = true;
                attConsumingService.ServiceName = new[] { new LocalizedName("SP", "en") };

                attConsumingService.RequestedAttribute = new RequestedAttribute[config.Metadata.RequestedAttributes.Count];

                for (var i = 0; i < config.Metadata.RequestedAttributes.Count; i++)
                {
                    attConsumingService.RequestedAttribute[i] = new RequestedAttribute
                                                                    {
                                                                        Name = config.Metadata.RequestedAttributes[i].Name
                                                                    };
                    if (config.Metadata.RequestedAttributes[i].IsRequired)
                    {
                        attConsumingService.RequestedAttribute[i].IsRequired = true;
                    }

                    attConsumingService.RequestedAttribute[i].NameFormat = SamlAttribute.NameformatBasic;
                }
            }
            else
            {
                serviceProviderDescriptor.AttributeConsumingService = new AttributeConsumingService[0];
            }

            if (config.Metadata == null || !config.Metadata.ExcludeArtifactEndpoints)
            {
                serviceProviderDescriptor.ArtifactResolutionService = artifactResolutionEndpoints.ToArray();
            }

            entity.Items = new object[] { serviceProviderDescriptor };

            // Keyinfo
            var keySigning = new KeyDescriptor();
            var keyEncryption = new KeyDescriptor();
            serviceProviderDescriptor.KeyDescriptor = new[] { keySigning, keyEncryption };
            
            keySigning.Use = KeyTypes.Signing;
            keySigning.UseSpecified = true;

            keyEncryption.Use = KeyTypes.Encryption;
            keyEncryption.UseSpecified = true;
                       
            // Ugly conversion between the .Net framework classes and our classes ... avert your eyes!!
            keySigning.KeyInfo = Serialization.DeserializeFromXmlString<Schema.XmlDSig.KeyInfo>(keyInfo.GetXml().OuterXml);            
            keyEncryption.KeyInfo = keySigning.KeyInfo;

            // apply the <Organization> element
            if (config.Metadata.Organization.ElementInformation.IsPresent)
            {
                entity.Organization = new Organization
                                          {
                                              OrganizationName = new[] { new LocalizedName { Value = config.Metadata.Organization.Name } },
                                              OrganizationDisplayName = new[] { new LocalizedName { Value = config.Metadata.Organization.DisplayName } },
                                              OrganizationURL = new[] { new LocalizedURI { Value = config.Metadata.Organization.Url } }
                                          };
            }

            if (config.Metadata.Contacts != null && config.Metadata.Contacts.Count > 0)
            {
                entity.ContactPerson = config.Metadata.Contacts.Select(x => new Contact
                                                                                {
                                                                                    ContactType =
                                                                                        (Schema.Metadata.ContactType)
                                                                                        ((int)x.Type),
                                                                                    Company = x.Company,
                                                                                    GivenName = x.GivenName,
                                                                                    SurName = x.SurName,
                                                                                    EmailAddress = new[] { x.Email },
                                                                                    TelephoneNumber = new[] { x.Phone }
                                                                                }).ToArray();
            }
        }

        /// <summary>
        /// Extracts the endpoints.
        /// </summary>
        private void ExtractEndpoints()
        {
            if (Entity != null)
            {
                _ssoEndpoints = new List<IdentityProviderEndpointElement>();
                _idpSloEndpoints = new List<IdentityProviderEndpointElement>();
                _idpArsEndpoints = new Dictionary<int, IndexedEndpoint>();
                _spSloEndpoints = new List<IdentityProviderEndpointElement>();
                _spArsEndpoints = new Dictionary<int, IndexedEndpoint>();
                _assertionConsumerServiceEndpoints = new List<IdentityProviderEndpointElement>();
                _attributeQueryEndpoints = new List<Endpoint>();

                foreach (var item in Entity.Items)
                {
                    if (item is IdpSsoDescriptor)
                    {
                        var descriptor = (IdpSsoDescriptor)item;
                        foreach (var endpoint in descriptor.SingleSignOnService)
                        {
                            BindingType binding;
                            switch (endpoint.Binding)
                            {
                                case Saml20Constants.ProtocolBindings.HttpPost:
                                    binding = BindingType.Post;
                                    break;
                                case Saml20Constants.ProtocolBindings.HttpRedirect:
                                    binding = BindingType.Redirect;
                                    break;
                                case Saml20Constants.ProtocolBindings.HttpArtifact:
                                    binding = BindingType.Artifact;
                                    break;
                                case Saml20Constants.ProtocolBindings.HttpSoap:
                                    binding = BindingType.Artifact;
                                    break;
                                default:
                                    throw new InvalidOperationException("Binding not supported: " + endpoint.Binding);
                            }

                            _ssoEndpoints.Add(new IdentityProviderEndpointElement { Url = endpoint.Location, Binding = binding });
                        }

                        if (descriptor.SingleLogoutService != null)
                        {
                            foreach (var endpoint in descriptor.SingleLogoutService)
                            {
                                BindingType binding;
                                switch (endpoint.Binding)
                                {
                                    case Saml20Constants.ProtocolBindings.HttpPost:
                                        binding = BindingType.Post;
                                        break;
                                    case Saml20Constants.ProtocolBindings.HttpRedirect:
                                        binding = BindingType.Redirect;
                                        break;
                                    case Saml20Constants.ProtocolBindings.HttpArtifact:
                                        binding = BindingType.Artifact;
                                        break;
                                    case Saml20Constants.ProtocolBindings.HttpSoap:
                                        binding = BindingType.Artifact;
                                        break;
                                    default:
                                        throw new InvalidOperationException("Binding not supported: " + endpoint.Binding);
                                }

                                _idpSloEndpoints.Add(new IdentityProviderEndpointElement { Url = endpoint.Location, Binding = binding });
                            }
                        }

                        if (descriptor.ArtifactResolutionService != null)
                        {
                            foreach (var ie in descriptor.ArtifactResolutionService)
                            {
                                _idpArsEndpoints.Add(ie.Index, ie);
                            }
                        }
                    }

                    if (item is SpSsoDescriptor)
                    {
                        var descriptor = (SpSsoDescriptor)item;
                        foreach (var endpoint in descriptor.AssertionConsumerService)
                        {
                            BindingType binding;
                            switch (endpoint.Binding)
                            {
                                case Saml20Constants.ProtocolBindings.HttpPost:
                                    binding = BindingType.Post;
                                    break;
                                case Saml20Constants.ProtocolBindings.HttpRedirect:
                                    binding = BindingType.Redirect;
                                    break;
                                case Saml20Constants.ProtocolBindings.HttpArtifact:
                                    binding = BindingType.Artifact;
                                    break;
                                case Saml20Constants.ProtocolBindings.HttpSoap:
                                    binding = BindingType.Artifact;
                                    break;
                                default:
                                    throw new InvalidOperationException("Binding not supported: " + endpoint.Binding);
                            }

                            _assertionConsumerServiceEndpoints.Add(new IdentityProviderEndpointElement { Url = endpoint.Location, Binding = binding });
                        }

                        if (descriptor.SingleLogoutService != null)
                        {
                            foreach (var endpoint in descriptor.SingleLogoutService)
                            {
                                BindingType binding;
                                switch (endpoint.Binding)
                                {
                                    case Saml20Constants.ProtocolBindings.HttpPost:
                                        binding = BindingType.Post;
                                        break;
                                    case Saml20Constants.ProtocolBindings.HttpRedirect:
                                        binding = BindingType.Redirect;
                                        break;
                                    case Saml20Constants.ProtocolBindings.HttpArtifact:
                                        binding = BindingType.Artifact;
                                        break;
                                    case Saml20Constants.ProtocolBindings.HttpSoap:
                                        binding = BindingType.Artifact;
                                        break;
                                    default:
                                        throw new InvalidOperationException("Binding not supported: " + endpoint.Binding);
                                }

                                _spSloEndpoints.Add(new IdentityProviderEndpointElement { Url = endpoint.Location, Binding = binding });
                            }
                        }

                        if (descriptor.ArtifactResolutionService != null)
                        {
                            foreach (var ie in descriptor.ArtifactResolutionService)
                            {
                                _spArsEndpoints.Add(ie.Index, ie);
                            }
                        }
                    }

                    if (item is AttributeAuthorityDescriptor)
                    {
                        var aad = (AttributeAuthorityDescriptor)item;
                        _attributeQueryEndpoints.AddRange(aad.AttributeService);
                    }
                }
            }
        }

        /// <summary>
        /// Extract KeyDescriptors from the metadata document represented by this instance.
        /// </summary>
        private void ExtractKeyDescriptors()
        {            
            if (_keys != null || Entity == null)
            {
                return;
            }

            _keys = new List<KeyDescriptor>();
            foreach (var keyDescriptor in Entity.Items.OfType<RoleDescriptor>().SelectMany(rd => rd.KeyDescriptor))
            {
                _keys.Add(keyDescriptor);
            }
        }

        /// <summary>
        /// Retrieves the key descriptors contained in the document
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void ExtractKeyDescriptors(XmlDocument doc)
        {            
            var list = doc.GetElementsByTagName(KeyDescriptor.ElementName, Saml20Constants.Metadata);
            _keys = new List<KeyDescriptor>(list.Count);

            foreach (XmlNode node in list)
            {
                _keys.Add(Serialization.DeserializeFromXmlString<KeyDescriptor>(node.OuterXml));
            }                        
        }

        #endregion
    }
}
