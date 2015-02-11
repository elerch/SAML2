using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using SAML2.Config;
using SAML2.Logging;
using SAML2.Bindings;
using SAML2;
using System.IO;
using SAML2.Utils;
using SAML2.Protocol;
using System.Collections.Generic;
using Owin.Security.Saml;
using System.Collections.Specialized;
using System.Runtime.ExceptionServices;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security;

namespace Owin
{
    internal class SamlLoginHandler
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly IInternalLogger Logger = LoggerProvider.LoggerFor(typeof(SamlLoginHandler));

        private Saml2Configuration configuration;
        private readonly Func<string, object> getFromCache;
        private readonly IDictionary<string, object> session;
        private readonly Action<string, object, DateTime> setInCache;

        /// <summary>
        /// Key used to save temporary session id
        /// </summary>
        public const string IdpTempSessionKey = "TempIDPId";


        /// <summary>
        /// Key used to override <c>ForceAuthn</c> setting
        /// </summary>
        public const string IdpForceAuthn = "IDPForceAuthn";

        /// <summary>
        /// Key used to override IsPassive setting
        /// </summary>
        public const string IdpIsPassive = "IDPIsPassive";
        private readonly SamlAuthenticationOptions options;


        /// <summary>
        /// Constructor for LoginHandler
        /// </summary>
        /// <param name="configuration">SamlConfiguration</param>
        /// <param name="getFromCache">May be null unless doing artifact binding, this function will be called for artifact resolution</param>
        public SamlLoginHandler(SamlAuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");
            this.options = options;
            configuration = options.Configuration;
            getFromCache = options.GetFromCache;
            setInCache = options.SetInCache;
            session = options.Session;
        }


        /// <summary>
        /// Invokes the login procedure (2nd leg of SP-Initiated login). Analagous to Saml20SignonHandler from ASP.Net DLL
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(IOwinContext context)
        {
            Logger.Debug(TraceMessages.SignOnHandlerCalled);
            ExceptionDispatchInfo authFailedEx = null;
            try {
                var messageReceivedNotification = new MessageReceivedNotification<SamlMessage, SamlAuthenticationOptions>(context, options)
                {
                    ProtocolMessage = new SamlMessage(context, configuration, null)
                };
                await options.Notifications.MessageReceived(messageReceivedNotification);
                if (messageReceivedNotification.HandledResponse) {
                    return; // GetHandledResponseTicket()
                }
                if (messageReceivedNotification.Skipped) {
                    return;
                }
                await HandleResponse(context);
                var assertion = context.Get<Saml20Assertion>("Saml2:assertion");
                var securityTokenReceivedNotification = new SecurityTokenReceivedNotification<SamlMessage, SamlAuthenticationOptions>(context, options)
                {
                    ProtocolMessage = new SamlMessage(context, configuration, assertion)
                };
                await options.Notifications.SecurityTokenReceived(securityTokenReceivedNotification);
                if (securityTokenReceivedNotification.HandledResponse) {
                    return; // GetHandledResponseTicket();
                }
                if (securityTokenReceivedNotification.Skipped) {
                    return; // null;
                }

                var ticket = await GetAuthenticationTicket(context);

                var securityTokenValidatedNotification = new SecurityTokenValidatedNotification<SamlMessage, SamlAuthenticationOptions>(context, options)
                {
                    AuthenticationTicket = ticket,
                    ProtocolMessage = new SamlMessage(context, configuration, assertion)
                };

                await options.Notifications.SecurityTokenValidated(securityTokenValidatedNotification);
                if (securityTokenValidatedNotification.HandledResponse) {
                    return; // GetHandledResponseTicket();
                }
                if (securityTokenValidatedNotification.Skipped) {
                    return; // null;
                }
                // Flow possible changes
                ticket = securityTokenValidatedNotification.AuthenticationTicket;

                // TODO: get this out to the cookie middleware somehow...
            }
            catch (Exception ex) {
                authFailedEx = ExceptionDispatchInfo.Capture(ex);
            }
            if (authFailedEx != null) {
                Logger.Error("Exception occurred while processing message: " + authFailedEx.SourceException);
                var message = new SamlMessage(context, configuration, context.Get<Saml20Assertion>("Saml2:assertion"));
                var authenticationFailedNotification = new AuthenticationFailedNotification<SamlMessage, SamlAuthenticationOptions>(context, options)
                {
                    ProtocolMessage = message,
                    Exception = authFailedEx.SourceException
                };
                await options.Notifications.AuthenticationFailed(authenticationFailedNotification);
                if (authenticationFailedNotification.HandledResponse) {
                    return;//GetHandledResponseTicket();
                }
                if (authenticationFailedNotification.Skipped) {
                    return; //null
                }

                authFailedEx.Throw();
            }
        }

        private Task<AuthenticationTicket> GetAuthenticationTicket(IOwinContext context)
        {
            var assertion = context.Get<Saml20Assertion>("Saml2:assertion");
            if (assertion == null)
                throw new InvalidOperationException("no assertion found with which to create a ticket");
            // TODO: Get this for real
            return Task.FromResult(new AuthenticationTicket(new System.Security.Claims.ClaimsIdentity(), new AuthenticationProperties()));
        }

        private Task HandleResponse(IOwinContext context)
        { 
            Action<Saml20Assertion> loginAction = a => DoSignOn(context, a);

            // Some IdP's are known to fail to set an actual value in the SOAPAction header
            // so we just check for the existence of the header field.
            if (context.Request.Headers.ContainsKey(SoapConstants.SoapAction)) {
                Utility.HandleSoap(
                    GetBuilder(context),
                    context.Request.Body,
                    configuration,
                    loginAction,
                    getFromCache,
                    setInCache,
                    session);
                return Task.FromResult((object)null);
            }

            var requestParams = context.Request.GetRequestParameters().ToNameValueCollection();
            if (!string.IsNullOrWhiteSpace(requestParams["SAMLart"])) {
                HandleArtifact(context);
            }

            var samlResponse = requestParams["SamlResponse"];
            if (!string.IsNullOrWhiteSpace(samlResponse)) {
                var assertion = Utility.HandleResponse(configuration, samlResponse, session, getFromCache, setInCache);
                loginAction(assertion);
            } else {
                if (configuration.CommonDomainCookie.Enabled && context.Request.Query["r"] == null
                    && requestParams["cidp"] == null) {
                    Logger.Debug(TraceMessages.CommonDomainCookieRedirectForDiscovery);
                    context.Response.Redirect(configuration.CommonDomainCookie.LocalReaderEndpoint);
                } else {
                    Logger.WarnFormat(ErrorMessages.UnauthenticatedAccess, context.Request.Uri.OriginalString);
                    SendRequest(context, configuration, requestParams);
                }
            }
            return Task.FromResult((object)null);
        }
        /// <summary>
        /// Send an authentication request to the IDP.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendRequest(IOwinContext context, Saml2Configuration config, NameValueCollection requestParams)
        {
            // See if the "ReturnUrl" - parameter is set.
            var returnUrl = context.Request.Query["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && session != null) {
                session["RedirectUrl"] = returnUrl;
            }

            var isRedirected = false;
            var selectionUtil = new IdpSelectionUtil(Logger);
            var idp = selectionUtil.RetrieveIDP(requestParams, context.Request.Query.ToNameValueCollection(), config, s => { context.Response.Redirect(s); isRedirected = true; });
            if (isRedirected) return;
            if (idp == null) {
                // Display a page to the user where she can pick the IDP
                Logger.DebugFormat(TraceMessages.IdentityProviderRedirect);
                throw new NotImplementedException("IdP Selection screen not implemented");
                //var page = new SelectSaml20IDP();
                //page.ProcessRequest(context);
                //return;
            }

            var authnRequest = Saml20AuthnRequest.GetDefault(config);
            TransferClient(idp, authnRequest, context, requestParams);
        }

        /// <summary>
        /// Transfers the client.
        /// </summary>
        /// <param name="identityProvider">The identity provider.</param>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        private void TransferClient(IdentityProvider identityProvider, Saml20AuthnRequest request, IOwinContext context, NameValueCollection requestParams)
        {
            IdentityProviderEndpoint destination = ConfigureRequest(identityProvider, request, context);

            switch (destination.Binding) {
            case BindingType.Redirect:
                Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpRedirect);

                var redirectBuilder = new HttpRedirectBindingBuilder
                {
                    SigningKey =  configuration.ServiceProvider.SigningCertificate.PrivateKey,
                    Request = request.GetXml().OuterXml
                };

                Logger.DebugFormat(TraceMessages.AuthnRequestSent, redirectBuilder.Request);

                var redirectLocation = request.Destination + "?" + redirectBuilder.ToQuery();
                context.Response.Redirect(redirectLocation);
                break;
            case BindingType.Post:
                Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpPost);

                var postBuilder = new HttpPostBindingBuilder(destination);

                // Honor the ForceProtocolBinding and only set this if it's not already set
                if (string.IsNullOrEmpty(request.ProtocolBinding)) {
                    request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpPost;
                }

                var requestXml = request.GetXml();
                XmlSignatureUtils.SignDocument(requestXml, request.Id, configuration.ServiceProvider.SigningCertificate);
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

                artifactBuilder.RedirectFromLogin(destination, request, requestParams["relayState"], (s, o) => setInCache(s, o, DateTime.MinValue));
                break;
            default:
                Logger.Error(ErrorMessages.EndpointBindingInvalid);
                throw new Saml20Exception(ErrorMessages.EndpointBindingInvalid);
            }
        }

        private IdentityProviderEndpoint ConfigureRequest(IdentityProvider identityProvider, Saml20AuthnRequest request, IOwinContext context)
        {
            // Set the last IDP we attempted to login at.
            if (session != null) {
                session[IdpTempSessionKey] = identityProvider.Id;
            }
            context.Set(IdpTempSessionKey, identityProvider.Id);

            // Determine which endpoint to use from the configuration file or the endpoint metadata.
            var destination = IdpSelectionUtil.DetermineEndpointConfiguration(BindingType.Redirect, identityProvider.Endpoints.DefaultSignOnEndpoint, identityProvider.Metadata.SSOEndpoints);
            request.Destination = destination.Url;

            if (identityProvider.ForceAuth) {
                request.ForceAuthn = true;
            }

            // Check isPassive status
            var isPassiveFlag = session != null ? session[IdpIsPassive] : null;
            if (isPassiveFlag != null && (bool)isPassiveFlag) {
                request.IsPassive = true;
                session[IdpIsPassive] = null;
            }

            if (identityProvider.IsPassive) {
                request.IsPassive = true;
            }

            // Check if request should forceAuthn
            var forceAuthnFlag = session != null ? session[IdpForceAuthn] : null;
            if (forceAuthnFlag != null && (bool)forceAuthnFlag) {
                request.ForceAuthn = true;
                session[IdpForceAuthn] = null;
            }

            // Check if protocol binding should be forced
            if (identityProvider.Endpoints.DefaultSignOnEndpoint != null) {
                if (!string.IsNullOrEmpty(identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding)) {
                    request.ProtocolBinding = identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding;
                }
            }

            Utility.AddExpectedResponse(request, session);

            return destination;
        }

        private void HandleArtifact(IOwinContext context)
        {
            var builder = GetBuilder(context);
            // TODO: Need params version of these!
            var inputStream = builder.ResolveArtifact(context.Request.Query["SAMLart"], context.Request.Query["relayState"], configuration);

            Utility.HandleSoap(builder, inputStream, configuration, a => DoSignOn(context, a), getFromCache, setInCache, session);
        }

        private HttpArtifactBindingBuilder GetBuilder(IOwinContext context)
        {
            return new HttpArtifactBindingBuilder(
                configuration,
                context.Response.Redirect,
                m => SendResponseMessage(m, context));
        }

        private static void SendResponseMessage(string message, IOwinContext context)
        {
            context.Response.ContentType = "text/xml";
            using (var writer = new StreamWriter(context.Response.Body)) {
                writer.Write(HttpSoapBindingBuilder.WrapInSoapEnvelope(message));
                writer.Flush();
                writer.Close();
            }
        }


        /// <summary>
        /// Handles executing the login.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assertion">The assertion.</param>
        private void DoSignOn(IOwinContext context, Saml20Assertion assertion)
        {
            context.Set("Saml2:assertion", assertion);
            // TODO: This needs to signal to OWIN that the user has logged in
            // User is now logged in at IDP specified in tmp
            //context.Items[IdpLoginSessionKey] = context.Session != null ? context.Session[IdpTempSessionKey] : context.Items[IdpTempSessionKey];
            //context.Items[IdpSessionIdKey] = assertion.SessionIndex;
            //context.Items[IdpNameIdFormat] = assertion.Subject.Format;
            //context.Items[IdpNameId] = assertion.Subject.Value;
            //context.Authentication.AuthenticationResponseGrant = new Microsoft.Owin.Security.AuthenticationResponseGrant()
            var subject = assertion.Subject ?? new SAML2.Schema.Core.NameId();
            Logger.DebugFormat(TraceMessages.SignOnProcessed, assertion.SessionIndex, subject.Value, subject.Format);
        }

    }
}