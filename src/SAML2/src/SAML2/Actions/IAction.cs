using System.Web;
using SAML2.Protocol;
using SAML2.Config;

namespace SAML2.Actions
{
    /// <summary>
    /// An implementation of the IAction interface can be called during login and logoff of the 
    /// SAML Connector framework in order to perform a specific action.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Action performed during SignOn.
        /// </summary>
        /// <param name="handler">The handler initiating the call.</param>
        /// <param name="context">The current http context.</param>
        /// <param name="assertion">The SAML assertion of the currently logged in user.</param>
        void SignOnAction(AbstractEndpointHandler handler, HttpContext context, Saml20Assertion assertion, Saml2Section config);

        /// <summary>
        /// Action performed during logout.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="context">The context.</param>
        /// <param name="idpInitiated">During IdP initiated logout some actions such as redirecting should not be performed</param>
        void LogoutAction(AbstractEndpointHandler handler, HttpContext context, bool idpInitiated);
    }
}
