using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using SAML2.Config;
using SAML2.Utils;
using SAML2.Bindings;
using SAML2.AspNet;

namespace SAML2.Protocol
{
    /// <summary>
    /// Base class for all SAML20 specific endpoints.
    /// </summary>
    public abstract class Saml20AbstractEndpointHandler : AbstractEndpointHandler
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

        /// <summary>
        /// Gets or sets a value indicating whether configuration has been validated
        /// </summary>
        public static bool Validated { get; set; }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public sealed override void ProcessRequest(HttpContext context)
        {
            try
            {
                CheckConfiguration();
                Handle(context);
            }
            catch (ThreadAbortException)
            {
                // This will swallow the ThreadAbortException automatically thrown by Response.Redirect, Response.End, and Server.Transfer
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Handles the selection of an IDP. If only one IDP is found, the user is automatically redirected to it.
        /// If several are found, and nothing indicates to which one the user should be sent, this method returns null.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public IdentityProvider RetrieveIDP(HttpContext context)
        { return RetrieveIDP(context, null); }

        /// <summary>
        /// Handles the selection of an IDP. If only one IDP is found, the user is automatically redirected to it.
        /// If several are found, and nothing indicates to which one the user should be sent, this method returns null.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="config">Configuration.  If null, configuration will be populated from application config</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public IdentityProvider RetrieveIDP(HttpContext context, Saml2Section config)
        {
            // If idpChoice is set, use it value
            if (!string.IsNullOrEmpty(context.Request.Params[IdpChoiceParameterName]))
            {
                Logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromQueryString, context.Request.Params[IdpChoiceParameterName]);
                var endPoint = config.IdentityProviders.FirstOrDefault(x => x.Id == context.Request.Params[IdpChoiceParameterName]);
                if (endPoint != null)
                {
                    return endPoint;
                }
            }

            // If we have a common domain cookie, use it's value
            // It must have been returned from the local common domain cookie reader endpoint.
            if (!string.IsNullOrEmpty(context.Request.QueryString["_saml_idp"]))
            {
                var cdc = new CommonDomainCookie(context.Request.QueryString["_saml_idp"]);
                if (cdc.IsSet)
                {
                    var endPoint = config.IdentityProviders.FirstOrDefault(x => x.Id == cdc.PreferredIDP);
                    if (endPoint != null)
                    {
                        Logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromCommonDomainCookie, cdc.PreferredIDP);
                        return endPoint;
                    }

                    Logger.WarnFormat(ErrorMessages.CommonDomainCookieIdentityProviderInvalid, cdc.PreferredIDP);
                }
            }

            // If there is only one configured IdentityProviderEndpointElement lets just use that
            if (config.IdentityProviders.Count == 1 && config.IdentityProviders[0].Metadata != null)
            {
                Logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromDefault, config.IdentityProviders[0].Name);
                return config.IdentityProviders[0];
            }

            // If one of the endpoints are marked with default, use that one
            var defaultIDP = config.IdentityProviders.FirstOrDefault(idp => idp.Default);
            if (defaultIDP != null)
            {
                Logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromDefault, defaultIDP.Id);
                return defaultIDP;
            }

            // In case an IDP selection url has been configured, redirect to that one.
            if (!string.IsNullOrEmpty(config.IdentityProviders.SelectionUrl))
            {
                Logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromSelection, config.IdentityProviders.SelectionUrl);
                context.Response.Redirect(config.IdentityProviders.SelectionUrl);
            }

            // If an IDPSelectionEvent handler is present, request the handler for an IDP endpoint to use.
            return IdpSelectionUtil.InvokeIDPSelectionEventHandler(config.IdentityProviders);
        }

        /// <summary>
        /// Looks through the Identity Provider configurations and
        /// </summary>
        /// <param name="idpId">The identity provider id.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public IdentityProvider RetrieveIDPConfiguration(string idpId)
        {
            return RetrieveIDPConfiguration(idpId, null);
        }
        /// <summary>
        /// Looks through the Identity Provider configurations and
        /// </summary>
        /// <param name="idpId">The identity provider id.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public IdentityProvider RetrieveIDPConfiguration(string idpId, Saml2Section config)
        {
            return config.IdentityProviders.FirstOrDefault(x => x.Id == idpId);
        }

        /// <summary>
        /// Determine which endpoint to use based on the protocol defaults, configuration data and metadata.
        /// </summary>
        /// <param name="defaultBinding">The binding to use if none has been specified in the configuration and the metadata allows all bindings.</param>
        /// <param name="config">The endpoint as described in the configuration. May be null.</param>
        /// <param name="metadata">A list of endpoints of the given type (e.g. SSO or SLO) that the metadata contains.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        internal static IdentityProviderEndpoint DetermineEndpointConfiguration(BindingType defaultBinding, IdentityProviderEndpoint config, List<IdentityProviderEndpoint> metadata)
        {
            var result = new IdentityProviderEndpoint { Binding = defaultBinding };

            // Determine which binding to use.
            if (config != null)
            {
                result.Binding = config.Binding;
            }
            else
            {
                // Verify that the metadata allows the default binding.
                var allowed = metadata.Exists(el => el.Binding == defaultBinding);
                if (!allowed)
                {
                    result.Binding = result.Binding == BindingType.Post
                                         ? BindingType.Redirect
                                         : BindingType.Post;
                }
            }

            if (config != null && !string.IsNullOrEmpty(config.Url))
            {
                result.Url = config.Url;
            }
            else
            {
                var endpoint = metadata.Find(el => el.Binding == result.Binding);
                if (endpoint == null)
                {
                    throw new FormatException(string.Format("No IdentityProvider supporting SAML binding {0} found in metadata", result.Binding));
                }

                result.Url = endpoint.Url;
            }

            return result;
        }

        /// <summary>
        /// Abstract handler function
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected abstract void Handle(HttpContext ctx);

        /// <summary>
        /// Checks the configuration elements and redirects to an error page if something is missing or wrong.
        /// </summary>
        private void CheckConfiguration()
        {
            if (Validated)
            {
                return;
            }

            Validated = BindingUtility.ValidateConfiguration(ConfigurationFactory.Instance.Configuration);
        }
    }
}
