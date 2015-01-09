using System.Web;
using System.Web.Security;
using SAML2.Identity;
using SAML2.Protocol;
using SAML2.Config;

namespace SAML2.Actions
{
    /// <summary>
    /// Handles setting Forms Authentication cookies.
    /// </summary>
    public class FormsAuthenticationAction : IAction
    {
        /// <summary>
        /// Backing field for name.
        /// </summary>
        private string _name = "FormsAuthentication";

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
            FormsAuthentication.SetAuthCookie(Saml20PrincipalCache.GetPrincipal().Identity.Name, false);  
        }

        /// <summary>
        /// Action performed during logout.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <param name="idpInitiated">During IdP initiated logout some actions such as redirecting should not be performed</param>
        public void LogoutAction(AbstractEndpointHandler handler, HttpContext context, bool idpInitiated)
        {
            FormsAuthentication.SignOut();
        }
    }
}
