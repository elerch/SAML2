using System;
using Microsoft.Owin;
using SAML2.Utils;
using System.Collections.Specialized;
using SAML2.Config;
using SAML2;
using SAML2.Bindings;
using SAML2.Protocol;
using Microsoft.IdentityModel.Protocols;

namespace Owin.Security.Saml
{
    public class SamlMessage : AuthenticationProtocolMessage
    {
        /// <summary>
        /// Parameter name for IDP choice
        /// </summary>
        public const string IdpChoiceParameterName = "cidp";

        /// <summary>
        /// Key used to override <c>ForceAuthn</c> setting
        /// </summary>
        public const string IdpForceAuthn = "IDPForceAuthn";

        /// <summary>
        /// Key used to override IsPassive setting
        /// </summary>
        public const string IdpIsPassive = "IDPIsPassive";

        /// <summary>
        /// Key used to save login session
        /// </summary>
        public const string IdpLoginSessionKey = "LoginIDPId";

        /// <summary>
        /// Key used to save the IDP name id in session context
        /// </summary>
        public const string IdpNameId = "IDPNameId";

        /// <summary>
        /// Used to save the name id format of the assertion
        /// </summary>
        public const string IdpNameIdFormat = "IDPNameIdFormat";

        /// <summary>
        /// Key used to save SessionId
        /// </summary>
        public const string IdpSessionIdKey = "IDPSessionID";

        /// <summary>
        /// Key used to save temporary session id
        /// </summary>
        public const string IdpTempSessionKey = "TempIDPId";

        private IFormCollection form;
        private IOwinContext context;
        private readonly Saml2Configuration config;

        public SamlMessage(IFormCollection form)
        {
            this.form = form;
        }

        public SamlMessage(IFormCollection form, IOwinContext context, SAML2.Config.Saml2Configuration config) : this(form)
        {
            this.context = context;
            this.config = config;
        }

        public SamlMessage(IOwinContext context, Saml2Configuration config, Saml20Assertion assertion) : this(null, context, config)
        {
            Assertion = assertion;
        }

        public string Reply { get; internal set; }
        public Saml20Assertion Assertion { get; private set; }

        public bool IsSignInMessage()
        {
            return false;
        }

        internal string GetToken()
        {
            throw new NotImplementedException();
        }

        public override string BuildRedirectUrl()
        {
            string rc = null;
            var logger = SAML2.Logging.LoggerProvider.LoggerFor(typeof(SamlMessage));
            var selectionUtil = new IdpSelectionUtil(logger);
            var allparams = BuildParams(form, context.Request.Query);
            var idp = selectionUtil.RetrieveIDP(allparams, BuildParams(context.Request.Query), config, s => rc = s);
            if (rc != null) return rc; // IDP selection screen
            if (idp == null) {
                // Display a page to the user where she can pick the IDP
                logger.DebugFormat(TraceMessages.IdentityProviderRedirect);
                throw new NotImplementedException("Selection of IDP not yet done (probably need a map call on middleware extension method)");
                //var page = new SelectSaml20IDP();
                //page.ProcessRequest(context);
                //return;
            }

            var authnRequest = Saml20AuthnRequest.GetDefault(config);
            return AuthnRequestForIdp(idp, authnRequest, context, config);
        }

        private string AuthnRequestForIdp(IdentityProvider identityProvider, Saml20AuthnRequest request, IOwinContext context, Saml2Configuration config)
        {
            var logger = SAML2.Logging.LoggerProvider.LoggerFor(typeof(SamlMessage));

            context.Set(IdpTempSessionKey, identityProvider.Id);

            // Determine which endpoint to use from the configuration file or the endpoint metadata.
            var destination = IdpSelectionUtil.DetermineEndpointConfiguration(BindingType.Redirect, identityProvider.Endpoints.DefaultSignOnEndpoint, identityProvider.Metadata.SSOEndpoints);
            request.Destination = destination.Url;

            if (identityProvider.ForceAuth) {
                request.ForceAuthn = true;
            }

            // Check isPassive status
            if (context.Get<bool>(IdpIsPassive)) {
                request.IsPassive = true;
            }

            if (identityProvider.IsPassive) {
                request.IsPassive = true;
            }

            // Check if request should forceAuthn            
            if (context.Get<bool>(IdpForceAuthn)) {
                request.ForceAuthn = true;
            }

            // Check if protocol binding should be forced
            if (identityProvider.Endpoints.DefaultSignOnEndpoint != null) {
                if (!string.IsNullOrEmpty(identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding)) {
                    request.ProtocolBinding = identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding;
                }
            }

            // Save request message id to session
            Utility.AddExpectedResponseId(request.Id);

            switch (destination.Binding) {
            case BindingType.Redirect:
                logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpRedirect);

                var redirectBuilder = new HttpRedirectBindingBuilder
                {
                    SigningKey = config.ServiceProvider.SigningCertificate.PrivateKey,
                    Request = request.GetXml().OuterXml
                };
                if (context.Authentication != null &&
                    context.Authentication.AuthenticationResponseChallenge != null &&
                    context.Authentication.AuthenticationResponseChallenge.Properties != null &&
                    context.Authentication.AuthenticationResponseChallenge.Properties.Dictionary != null &&
                    context.Authentication.AuthenticationResponseChallenge.Properties.Dictionary.Count > 0)
                    redirectBuilder.RelayState = context.Authentication.AuthenticationResponseChallenge.Properties.Dictionary.ToDelimitedString();
                logger.DebugFormat(TraceMessages.AuthnRequestSent, redirectBuilder.Request);

                var redirectLocation = request.Destination + (request.Destination.Contains("?") ? "&" : "?") + redirectBuilder.ToQuery();
                return redirectLocation;
            case BindingType.Post:
                throw new NotImplementedException();
                //logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpPost);

                //var postBuilder = new HttpPostBindingBuilder(destination);

                //// Honor the ForceProtocolBinding and only set this if it's not already set
                //if (string.IsNullOrEmpty(request.ProtocolBinding)) {
                //    request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpPost;
                //}

                //var requestXml = request.GetXml();
                //XmlSignatureUtils.SignDocument(requestXml, request.Id, config.ServiceProvider.SigningCertificate);
                //postBuilder.Request = requestXml.OuterXml;

                //logger.DebugFormat(TraceMessages.AuthnRequestSent, postBuilder.Request);

                //context.Response.Write(postBuilder.GetPage());
                //break;
            case BindingType.Artifact:
                throw new NotImplementedException();
                //logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpArtifact);

                //var artifactBuilder = new HttpArtifactBindingBuilder(context, config);

                //// Honor the ForceProtocolBinding and only set this if it's not already set
                //if (string.IsNullOrEmpty(request.ProtocolBinding)) {
                //    request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpArtifact;
                //}

                //logger.DebugFormat(TraceMessages.AuthnRequestSent, request.GetXml().OuterXml);

                //artifactBuilder.RedirectFromLogin(destination, request);
                //break;
            default:
                logger.Error(SAML2.ErrorMessages.EndpointBindingInvalid);
                throw new Saml20Exception(SAML2.ErrorMessages.EndpointBindingInvalid);
            }
            throw new NotImplementedException();
        }

        private static NameValueCollection BuildParams(IReadableStringCollection query)
        {
            var nvc = new NameValueCollection();
            foreach (var item in query)
                nvc[item.Key] = item.Value[0];
            return nvc;

        }
        private static NameValueCollection BuildParams(IFormCollection form, IReadableStringCollection query)
        {
            var nvc = new NameValueCollection();
            if (form != null)
                foreach (var item in form)
                    nvc[item.Key] = item.Value[0];
            foreach (var item in query)
                nvc[item.Key] = item.Value[0];
            return nvc;

        }

        public override string BuildFormPost()
        {
            return base.BuildFormPost(); // See Saml20SignonHandler.cs, line 591 (post binding)
        }
    }
}