using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using SAML2.Bindings;
using SAML2.Config;
using SAML2.Protocol.Pages;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;
using SAML2.Utils;
using SAML2.AspNet;
using System.Web.SessionState;

namespace SAML2.Protocol
{
    /// <summary>
    /// Implements a SAML 2.0 protocol sign-on endpoint. Handles all SAML bindings.
    /// </summary>
    public class Saml20SignonHandler : Saml20AbstractEndpointHandler
    {
        /// <summary>
        /// The certificate for the endpoint.
        /// </summary>
        private readonly X509Certificate2 _certificate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20SignonHandler"/> class.
        /// </summary>
        public Saml20SignonHandler() : this(null)
        { }

        public Saml20SignonHandler(Saml2Configuration config)
        {
            _certificate = config.ServiceProvider.SigningCertificate;

            // Read the proper redirect url from config
            try {
                RedirectUrl = config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.RedirectUrl;
            }
            catch (Exception e) {
                Logger.Error(e.Message, e);
            }
        }

        protected override void Handle(HttpContext context)
        {
            Handle(context, ConfigurationFactory.Instance.Configuration);
        }
        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Handle(HttpContext context, Saml2Configuration config)
        {
            Logger.Debug(TraceMessages.SignOnHandlerCalled);
            var getFromCache = new Func<string, object>(context.Cache.Get);
            var setInCache = new Action<string, object, DateTime>((s, o, d) => context.Cache.Insert(s, o, null, d, Cache.NoSlidingExpiration));
            var loginAction = new Action<Saml20Assertion>(a => DoSignOn(context, a, config));
            var session = SessionToDictionary(context.Session);

            // Some IdP's are known to fail to set an actual value in the SOAPAction header
            // so we just check for the existence of the header field.
            if (Array.Exists(context.Request.Headers.AllKeys, s => s == SoapConstants.SoapAction)) {
                Utility.HandleSoap(
                    GetBuilder(context),
                    context.Request.InputStream,
                    config,
                    loginAction,
                    getFromCache,
                    setInCache,
                    session);
                return;
            }

            if (!string.IsNullOrEmpty(context.Request.Params["SAMLart"])) {
                HandleArtifact(context, config, (c, s, conf) =>
                    Utility.HandleSoap(
                        GetBuilder(context),
                        s,
                        conf,
                        loginAction,
                        getFromCache,
                        setInCache,
                        session));
            }
            
            var samlResponse = context.Request.Params["SamlResponse"];
            if (!string.IsNullOrEmpty(samlResponse)) {
                var assertion = Utility.HandleResponse(config, samlResponse, session, getFromCache, setInCache);
                loginAction(assertion);
            } else {
                if (config.CommonDomainCookie.Enabled && context.Request.QueryString["r"] == null
                    && context.Request.Params["cidp"] == null) {
                    Logger.Debug(TraceMessages.CommonDomainCookieRedirectForDiscovery);
                    context.Response.Redirect(config.CommonDomainCookie.LocalReaderEndpoint);
                } else {
                    Logger.WarnFormat(ErrorMessages.UnauthenticatedAccess, context.Request.RawUrl);
                    SendRequest(context, config);
                }
            }
        }

        /// <summary>
        /// Handles executing the login.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assertion">The assertion.</param>
        private void DoSignOn(HttpContext context, Saml20Assertion assertion, Saml2Configuration config)
        {
            // User is now logged in at IDP specified in tmp
            context.Items[IdpLoginSessionKey] = context.Session != null ? context.Session[IdpTempSessionKey] : context.Items[IdpTempSessionKey];
            context.Items[IdpSessionIdKey] = assertion.SessionIndex;
            context.Items[IdpNameIdFormat] = assertion.Subject.Format;
            context.Items[IdpNameId] = assertion.Subject.Value;

            Logger.DebugFormat(TraceMessages.SignOnProcessed, assertion.SessionIndex, assertion.Subject.Value, assertion.Subject.Format);

            Logger.Debug(TraceMessages.SignOnActionsExecuting);
            // TODO: Signon event
            //foreach (var action in Actions.Actions.GetActions(config))
            //{
            //    Logger.DebugFormat("{0}.{1} called", action.GetType(), "LoginAction()");

            //    action.SignOnAction(this, context, assertion, config);

            //    Logger.DebugFormat("{0}.{1} finished", action.GetType(), "LoginAction()");
            //}
        }

        private static IDictionary<string, object> SessionToDictionary(HttpSessionState session)
        {
            if (session == null) return null;
            return session.Keys.AsQueryable().Cast<string>().ToDictionary(s => s, s => session[s]);
        }

        /// <summary>
        /// Send an authentication request to the IDP.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendRequest(HttpContext context, Saml2Configuration config)
        {
            // See if the "ReturnUrl" - parameter is set.
            var returnUrl = context.Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && context.Session != null) {
                context.Session["RedirectUrl"] = returnUrl;
            }

            var isRedirected = false;
            var selectionUtil = new IdpSelectionUtil(Logger);
            var idp = selectionUtil.RetrieveIDP(context.Request.Params, context.Request.QueryString, config, s => { context.Response.Redirect(s); isRedirected = true; });
            if (isRedirected) return;
            if (idp == null) {
                // Display a page to the user where she can pick the IDP
                Logger.DebugFormat(TraceMessages.IdentityProviderRedirect);

                var page = new SelectSaml20IDP();
                page.ProcessRequest(context);
                return;
            }

            var authnRequest = Saml20AuthnRequest.GetDefault(config);
            TransferClient(idp, authnRequest, context, config);
        }

        /// <summary>
        /// Transfers the client.
        /// </summary>
        /// <param name="identityProvider">The identity provider.</param>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        private void TransferClient(IdentityProvider identityProvider, Saml20AuthnRequest request, HttpContext context, Saml2Configuration config)
        {
            IdentityProviderEndpoint destination = ConfigureRequest(identityProvider, request, context);

            switch (destination.Binding) {
            case BindingType.Redirect:
                Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpRedirect);

                var redirectBuilder = new HttpRedirectBindingBuilder
                {
                    SigningKey = _certificate.PrivateKey,
                    Request = request.GetXml().OuterXml
                };

                Logger.DebugFormat(TraceMessages.AuthnRequestSent, redirectBuilder.Request);

				var redirectLocation = string.Format( "{0}{1}{2}", request.Destination, ( request.Destination.EndsWith( "?" ) ? "&" : "?" ), redirectBuilder.ToQuery() );
                context.Response.Redirect(redirectLocation, true);
                break;
            case BindingType.Post:
                Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpPost);

                var postBuilder = new HttpPostBindingBuilder(destination);

                // Honor the ForceProtocolBinding and only set this if it's not already set
                if (string.IsNullOrEmpty(request.ProtocolBinding)) {
                    request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpPost;
                }

                var requestXml = request.GetXml();
                XmlSignatureUtils.SignDocument(requestXml, request.Id, config.ServiceProvider.SigningCertificate);
                postBuilder.Request = requestXml.OuterXml;

                Logger.DebugFormat(TraceMessages.AuthnRequestSent, postBuilder.Request);

                context.Response.Write(postBuilder.GetPage());
                break;
            case BindingType.Artifact:
                Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpArtifact);

                var artifactBuilder = GetBuilder(context);

                // Honor the ForceProtocolBinding and only set this if it's not already set
                if (string.IsNullOrEmpty(request.ProtocolBinding)) {
                    request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpArtifact;
                }

                Logger.DebugFormat(TraceMessages.AuthnRequestSent, request.GetXml().OuterXml);

                artifactBuilder.RedirectFromLogin(destination, request, context.Request.Params["relayState"], (s, o) => context.Cache.Insert(s, o, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration));
                break;
            default:
                Logger.Error(ErrorMessages.EndpointBindingInvalid);
                throw new Saml20Exception(ErrorMessages.EndpointBindingInvalid);
            }
        }

        private static IdentityProviderEndpoint ConfigureRequest(IdentityProvider identityProvider, Saml20AuthnRequest request, HttpContext context)
        {
            // Set the last IDP we attempted to login at.
            if (context.Session != null) {
                context.Session[IdpTempSessionKey] = identityProvider.Id;
            }
            context.Items[IdpTempSessionKey] = identityProvider.Id;

            // Determine which endpoint to use from the configuration file or the endpoint metadata.
            var destination = IdpSelectionUtil.DetermineEndpointConfiguration(BindingType.Redirect, identityProvider.Endpoints.DefaultSignOnEndpoint, identityProvider.Metadata.SSOEndpoints);
            request.Destination = destination.Url;

            if (identityProvider.ForceAuth) {
                request.ForceAuthn = true;
            }

            // Check isPassive status
            var isPassiveFlag = context.Session != null ? context.Session[IdpIsPassive] : null;
            if (isPassiveFlag != null && (bool)isPassiveFlag) {
                request.IsPassive = true;
                context.Session[IdpIsPassive] = null;
            }

            if (identityProvider.IsPassive) {
                request.IsPassive = true;
            }

            // Check if request should forceAuthn
            var forceAuthnFlag = context.Session != null ? context.Session[IdpForceAuthn] : null;
            if (forceAuthnFlag != null && (bool)forceAuthnFlag) {
                request.ForceAuthn = true;
                context.Session[IdpForceAuthn] = null;
            }

            // Check if protocol binding should be forced
            if (identityProvider.Endpoints.DefaultSignOnEndpoint != null) {
                if (!string.IsNullOrEmpty(identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding)) {
                    request.ProtocolBinding = identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding;
                }
            }

            Utility.AddExpectedResponse(request, SessionToDictionary(context.Session));

            return destination;
        }
    }
}
