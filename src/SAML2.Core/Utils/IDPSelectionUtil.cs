using SAML2.Config;
using SAML2.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SAML2.Utils
{
    /// <summary>
    /// This delegate is used handling events, where the framework have several configured IDP's to choose from
    /// and needs information on, which one to use.
    /// </summary>
    /// <param name="ep">List of configured endpoints</param>
    /// <returns>The <see cref="IdentityProviderEndpoint"/> for the IDP that should be used for authentication</returns>
    public delegate IdentityProvider IdpSelectionEventHandler(IdentityProviders ep);

    /// <summary>
    /// Contains helper functionality for selection of IDP when more than one is configured
    /// </summary>
    public class IdpSelectionUtil
    {
        public const string IdpChoiceParameterName = "cidp";

        private readonly IInternalLogger logger;

        public IdpSelectionUtil(IInternalLogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }
        /// <summary>
        /// The event handler will be called, when no Common Domain Cookie is set, 
        /// no IdentityProviderEndpointElement is marked as default in the configuration,
        /// and no <c>idpSelectionUrl</c> is configured.
        /// Make sure that only one event handler is added, since only the last result of the event handler invocation will be used.
        /// </summary>
        public static event IdpSelectionEventHandler IdpSelectionEvent;

        /// <summary>
        /// Helper method for generating URL to a link, that the user can click to select that particular IdentityProviderEndpointElement for authorization.
        /// Usually not called directly, but called from <c>IdentityProviderEndpointElement.GetIDPLoginUrl()</c>
        /// </summary>
        /// <param name="idpId">Id of IDP that an authentication URL is needed for</param>
        /// <returns>A URL that can be used for logging in at the IDP</returns>
        public static string GetIdpLoginUrl(string idpId, Saml2Configuration config)
        {
            throw new System.NotImplementedException();
            // TODO: This should be needed eventually, but probably has to be handled outside of core
            //return string.Format("{0}?{1}={2}", config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.LocalPath, Saml20SignonHandler.IdpChoiceParameterName, HttpUtility.UrlEncode(idpId));
        }

        /// <summary>
        /// Invokes the IDP selection event handler.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public static IdentityProvider InvokeIDPSelectionEventHandler(IdentityProviders endpoints)
        {
            return IdpSelectionEvent != null ? IdpSelectionEvent(endpoints) : null;
        }

        /// <summary>
        /// Looks through the Identity Provider configurations and
        /// </summary>
        /// <param name="idpId">The identity provider id.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public static IdentityProvider RetrieveIDPConfiguration(string idpId, Saml2Configuration config)
        {
            return config.IdentityProviders.FirstOrDefault(x => x.Id == idpId);
        }

        /// <summary>
        /// Handles the selection of an IDP. If only one IDP is found, the user is automatically redirected to it.
        /// If several are found, and nothing indicates to which one the user should be sent, this method returns null.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="config">Configuration.  If null, configuration will be populated from application config</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public IdentityProvider RetrieveIDP(NameValueCollection allparams, NameValueCollection queryString, Saml2Configuration config, Action<string> redirectToSelection)
        {
            // If idpChoice is set, use it value
            if (!string.IsNullOrEmpty(allparams[IdpChoiceParameterName])) {
                logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromQueryString, allparams[IdpChoiceParameterName]);
                var endPoint = config.IdentityProviders.FirstOrDefault(x => x.Id == allparams[IdpChoiceParameterName]);
                if (endPoint != null) {
                    return endPoint;
                }
            }

            // If we have a common domain cookie, use it's value
            // It must have been returned from the local common domain cookie reader endpoint.
            if (!string.IsNullOrEmpty(queryString["_saml_idp"])) {
                var cdc = new Protocol.CommonDomainCookie(queryString["_saml_idp"]);
                if (cdc.IsSet) {
                    var endPoint = config.IdentityProviders.FirstOrDefault(x => x.Id == cdc.PreferredIDP);
                    if (endPoint != null) {
                        logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromCommonDomainCookie, cdc.PreferredIDP);
                        return endPoint;
                    }

                    logger.WarnFormat(ErrorMessages.CommonDomainCookieIdentityProviderInvalid, cdc.PreferredIDP);
                }
            }

            // If there is only one configured IdentityProviderEndpointElement lets just use that
            if (config.IdentityProviders.Count == 1 && config.IdentityProviders[0].Metadata != null) {
                logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromDefault, config.IdentityProviders[0].Name);
                return config.IdentityProviders[0];
            }

            // If one of the endpoints are marked with default, use that one
            var defaultIDP = config.IdentityProviders.FirstOrDefault(idp => idp.Default);
            if (defaultIDP != null) {
                logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromDefault, defaultIDP.Id);
                return defaultIDP;
            }

            // In case an IDP selection url has been configured, redirect to that one.
            if (!string.IsNullOrEmpty(config.IdentityProviders.SelectionUrl)) {
                logger.DebugFormat(TraceMessages.IdentityProviderRetreivedFromSelection, config.IdentityProviders.SelectionUrl);
                redirectToSelection(config.IdentityProviders.SelectionUrl);
                return null;
            }

            // If an IDPSelectionEvent handler is present, request the handler for an IDP endpoint to use.
            return IdpSelectionUtil.InvokeIDPSelectionEventHandler(config.IdentityProviders);
        }

        /// <summary>
        /// Determine which endpoint to use based on the protocol defaults, configuration data and metadata.
        /// </summary>
        /// <param name="defaultBinding">The binding to use if none has been specified in the configuration and the metadata allows all bindings.</param>
        /// <param name="config">The endpoint as described in the configuration. May be null.</param>
        /// <param name="metadata">A list of endpoints of the given type (e.g. SSO or SLO) that the metadata contains.</param>
        /// <returns>The <see cref="IdentityProvider"/>.</returns>
        public static IdentityProviderEndpoint DetermineEndpointConfiguration(BindingType defaultBinding, IdentityProviderEndpoint config, List<IdentityProviderEndpoint> metadata)
        {
            var result = new IdentityProviderEndpoint { Binding = defaultBinding };

            // Determine which binding to use.
            if (config != null) {
                result.Binding = config.Binding;
            } else {
                // Verify that the metadata allows the default binding.
                var allowed = metadata.Exists(el => el.Binding == defaultBinding);
                if (!allowed) {
                    result.Binding = result.Binding == BindingType.Post
                                         ? BindingType.Redirect
                                         : BindingType.Post;
                }
            }

            if (config != null && !string.IsNullOrEmpty(config.Url)) {
                result.Url = config.Url;
            } else {
                var endpoint = metadata.Find(el => el.Binding == result.Binding);
                if (endpoint == null) {
                    throw new FormatException(string.Format("No IdentityProvider supporting SAML binding {0} found in metadata", result.Binding));
                }

                result.Url = endpoint.Url;
            }

            return result;
        }
    }
}
