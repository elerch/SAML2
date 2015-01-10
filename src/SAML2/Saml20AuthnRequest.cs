using System;
using System.Collections.Generic;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Encapsulates a SAML 2.0 authentication request
    /// </summary>
    public class Saml20AuthnRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20AuthnRequest"/> class.
        /// </summary>
        public Saml20AuthnRequest()
        {
            Request = new AuthnRequest
                          {
                              Version = Saml20Constants.Version,
                              Id = "id" + Guid.NewGuid().ToString("N"),
                              Issuer = new NameId(),
                              IssueInstant = DateTime.Now
                          };
        }

        #region Request properties

        /// <summary>
        /// Gets or sets the assertion consumer service URL.
        /// </summary>
        /// <value>The assertion consumer service URL.</value>
        public string AssertionConsumerServiceUrl
        {
            get { return Request.AssertionConsumerServiceUrl; }
            set { Request.AssertionConsumerServiceUrl = value; }
        }

        /// <summary>
        /// Gets the underlying schema class object.
        /// </summary>
        /// <value>The request.</value>
        public AuthnRequest Request { get; private set; }

        /// <summary>
        /// Gets or sets the <c>Destination</c> attribute of the <c>AuthnRequest</c>.
        /// </summary>
        public string Destination
        {
            get { return Request.Destination; }
            set { Request.Destination = value; }
        }

        /// <summary>
        /// Gets or sets the <c>ForceAuthn</c> attribute of the <c>AuthnRequest</c>.
        /// </summary>
        public bool? ForceAuthn
        {
            get { return Request.ForceAuthn; }
            set { Request.ForceAuthn = value; }
        }

        /// <summary>
        /// Gets or sets the ID attribute of the <c>AuthnRequest</c> message.
        /// </summary>
        public string Id
        {
            get { return Request.Id; }
            set { Request.Id = value; }
        }

        /// <summary>
        /// Gets or sets the 'IsPassive' attribute of the <c>AuthnRequest</c>.
        /// </summary>
        public bool? IsPassive
        {
            get { return Request.IsPassive; }
            set { Request.IsPassive = value; }
        }

        /// <summary>
        /// Gets or sets the <c>IssueInstant</c> of the <c>AuthnRequest</c>.
        /// </summary>
        /// <value>The issue instant.</value>
        public DateTime? IssueInstant
        {
            get { return Request.IssueInstant; }
            set { Request.IssueInstant = value; }
        }

        /// <summary>
        /// Gets or sets the issuer value.
        /// </summary>
        /// <value>The issuer value.</value>
        public string Issuer
        {
            get { return Request.Issuer.Value; }
            set { Request.Issuer.Value = value; }
        }

        /// <summary>
        /// Gets or sets the issuer format.
        /// </summary>
        /// <value>The issuer format.</value>
        public string IssuerFormat
        {
            get { return Request.Issuer.Format; }
            set { Request.Issuer.Format = value; }
        }

        /// <summary>
        /// Gets or sets the name ID policy.
        /// </summary>
        /// <value>The name ID policy.</value>
        public NameIdPolicy NameIdPolicy
        {
            get { return Request.NameIdPolicy; }
            set { Request.NameIdPolicy = value; }
        }

        /// <summary>
        /// Gets or sets the ProtocolBinding on the request
        /// </summary>
        public string ProtocolBinding
        {
            get { return Request.ProtocolBinding; }
            set { Request.ProtocolBinding = value; }
        }

        /// <summary>
        /// Gets or sets the requested authentication context.
        /// </summary>
        /// <value>The requested authentication context.</value>
        public RequestedAuthnContext RequestedAuthnContext
        {
            get { return Request.RequestedAuthnContext; }
            set { Request.RequestedAuthnContext = value; }
        }

        #endregion

        public static Saml20AuthnRequest GetDefault()
        {
            return GetDefault(null);
        }
        /// <summary>
        /// Returns an instance of the class with meaningful default values set.
        /// </summary>
        /// <returns>The default <see cref="Saml20AuthnRequest"/>.</returns>
        public static Saml20AuthnRequest GetDefault(Saml2Section config)
        {
            config = config ?? Saml2Config.GetConfig();
            var result = new Saml20AuthnRequest { Issuer = config.ServiceProvider.Id };
            if (config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.Binding != BindingType.NotSet)
            {
                var baseUrl = new Uri(config.ServiceProvider.Server);
                result.AssertionConsumerServiceUrl = new Uri(baseUrl, config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.LocalPath).ToString();
            }

            // Binding
            switch (config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.Binding)
            {
                case BindingType.Artifact:
                    result.Request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpArtifact;
                    break;
                case BindingType.Post:
                    result.Request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpPost;
                    break;
                case BindingType.Redirect:
                    result.Request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpRedirect;
                    break;
                case BindingType.Soap:
                    result.Request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpSoap;
                    break;
            }

            // NameIDPolicy
            if (config.ServiceProvider.NameIdFormats.Count > 0)
            {
                result.NameIdPolicy = new NameIdPolicy
                                          {
                                              AllowCreate = config.ServiceProvider.NameIdFormats.AllowCreate,
                                              Format = config.ServiceProvider.NameIdFormats[0].Format
                                          };

                if (result.NameIdPolicy.Format != Saml20Constants.NameIdentifierFormats.Entity)
                {
                    result.NameIdPolicy.SPNameQualifier = config.ServiceProvider.Id;
                }
            }

            // RequestedAuthnContext
            if (config.ServiceProvider.AuthenticationContexts.Count > 0)
            {
                result.RequestedAuthnContext = new RequestedAuthnContext();
                switch (config.ServiceProvider.AuthenticationContexts.Comparison)
                {
                    case AuthenticationContextComparison.Better:
                        result.RequestedAuthnContext.Comparison = AuthnContextComparisonType.Better;
                        result.RequestedAuthnContext.ComparisonSpecified = true;
                        break;
                    case AuthenticationContextComparison.Minimum:
                        result.RequestedAuthnContext.Comparison = AuthnContextComparisonType.Minimum;
                        result.RequestedAuthnContext.ComparisonSpecified = true;
                        break;
                    case AuthenticationContextComparison.Maximum:
                        result.RequestedAuthnContext.Comparison = AuthnContextComparisonType.Maximum;
                        result.RequestedAuthnContext.ComparisonSpecified = true;
                        break;
                    case AuthenticationContextComparison.Exact:
                        result.RequestedAuthnContext.Comparison = AuthnContextComparisonType.Exact;
                        result.RequestedAuthnContext.ComparisonSpecified = true;
                        break;
                    default:
                        result.RequestedAuthnContext.ComparisonSpecified = false;
                        break;
                }

                result.RequestedAuthnContext.Items = new string[config.ServiceProvider.AuthenticationContexts.Count];
                result.RequestedAuthnContext.ItemsElementName = new Schema.Protocol.AuthnContextType[config.ServiceProvider.AuthenticationContexts.Count];

                var count = 0;
                foreach (var authenticationContext in config.ServiceProvider.AuthenticationContexts)
                {
                    result.RequestedAuthnContext.Items[count] = authenticationContext.Context;
                    switch (authenticationContext.ReferenceType)
                    {
                        case "AuthnContextDeclRef":
                            result.RequestedAuthnContext.ItemsElementName[count] = Schema.Protocol.AuthnContextType.AuthnContextDeclRef;
                            break;
                        default:
                            result.RequestedAuthnContext.ItemsElementName[count] = Schema.Protocol.AuthnContextType.AuthnContextClassRef;
                            break;
                    }

                    count++;
                }
            }

            // Restrictions
            var audienceRestrictions = new List<ConditionAbstract>(1);
            var audienceRestriction = new AudienceRestriction { Audience = new List<string>(1) { config.ServiceProvider.Id } };
            audienceRestrictions.Add(audienceRestriction);

            result.SetConditions(audienceRestrictions);

            return result;
        }

        /// <summary>
        /// Returns the <c>AuthnRequest</c> as an XML document.
        /// </summary>
        /// <returns>The request XML.</returns>
        public XmlDocument GetXml()
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(Serialization.SerializeToXmlString(Request));

            return doc;
        }

        /// <summary>
        /// Sets the conditions.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        private void SetConditions(List<ConditionAbstract> conditions)
        {
            Request.Conditions = new Conditions { Items = conditions };
        }
    }
}
