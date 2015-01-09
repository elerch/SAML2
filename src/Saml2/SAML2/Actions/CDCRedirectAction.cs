using System;
using System.Linq;
using System.Web;
using SAML2.Protocol;
using SAML2.Config;

namespace SAML2.Actions
{
    /// <summary>
    /// This action redirects to a Common Domain Cookie writer endpoint at the IdP.
    /// </summary>
    public class CDCRedirectAction : IAction
    {
        /// <summary>
        /// setting name for the identity provider cookie writer url 
        /// </summary>
        public const string IDPCookieWriterEndPoint = "idpCookieWriterEndPoint";

        /// <summary>
        /// Local return url setting name
        /// </summary>
        public const string LocalReturnUrl = "localReturnUrl";

        /// <summary>
        /// TargetResource query string parameter name.
        /// </summary>
        public const string TargetResource = "TargetResource";

        /// <summary>
        /// Name backing field.
        /// </summary>
        private string _name = "CDCRedirectAction";

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Action performed during SignOn.
        /// </summary>
        /// <param name="handler">The handler initiating the call.</param>
        /// <param name="context">The current http context.</param>
        /// <param name="assertion">The SAML assertion of the currently logged in user.</param>
        public void SignOnAction(AbstractEndpointHandler handler, HttpContext context, Saml20Assertion assertion, Saml2Section config)
        {
            var idpKey = (string)context.Session[Saml20SignonHandler.IdpLoginSessionKey];
            var signOnHandler = handler as Saml20SignonHandler;
            if (signOnHandler == null)
            {
                throw new ArgumentException("Endpoint handler must be of type Saml20SignonHandler.", "handler");
            }

            var identityProvider = signOnHandler.RetrieveIDPConfiguration(idpKey);
            if (identityProvider.CommonDomainCookie != null)
            {
                var values = identityProvider.CommonDomainCookie.AllKeys;

                var idpEndpoint = values.FirstOrDefault(x => x == IDPCookieWriterEndPoint);
                if (idpEndpoint == null)
                {
                    throw new Saml20Exception(@"Please specify """ + IDPCookieWriterEndPoint + @""" in CommonDomainCookie element.");
                }
                
                var localReturnPoint = values.FirstOrDefault(x => x == LocalReturnUrl);
                if (localReturnPoint == null)
                {
                    throw new Saml20Exception(@"Please specify """ + LocalReturnUrl + @""" in CommonDomainCookie element.");
                }

                context.Response.Redirect(idpEndpoint + "?" + TargetResource + "=" + localReturnPoint);
            }
            else
            {
                handler.DoRedirect(context);
            }
        }

        /// <summary>
        /// Action performed during logout.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <param name="idpInitiated">if set to <c>true</c> IDP is initiated.</param>
        public void LogoutAction(AbstractEndpointHandler handler, HttpContext context, bool idpInitiated)
        {
            if (!idpInitiated)
            {
                handler.DoRedirect(context);
            }
        }
    }
}
