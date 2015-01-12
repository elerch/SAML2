using System;
using System.Linq;
using System.Web;
using SAML2.Config;
using SAML2.AspNet;

namespace SAML2.Protocol
{
    /// <summary>
    /// SAML 2 common domain cookie Id return point.
    /// </summary>
    public class Saml20CDCIdpReturnPoint : AbstractEndpointHandler
    {
        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public override void ProcessRequest(HttpContext context)
        {
            Logger.DebugFormat("{0}.{1} called", GetType(), "ProcessRequest()");
                
            var config = ConfigurationFactory.Instance.Configuration;
            if (config == null)
            {
                throw new Saml20Exception(ErrorMessages.ConfigMissingSaml2Element);
            }

            var endp = config.ServiceProvider.Endpoints.FirstOrDefault(ep => ep.Type == EndpointType.SignOn);
            if (endp == null)
            {
                throw new Saml20Exception(ErrorMessages.ConfigServiceProviderMissingSignOnEndpoint);
            }

            var redirectUrl = (string)context.Session["RedirectUrl"];
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                context.Session.Remove("RedirectUrl");
                context.Response.Redirect(redirectUrl);
            }
            else if (string.IsNullOrEmpty(endp.RedirectUrl))
            {
                context.Response.Redirect("~/");
            }
            else
            {
                context.Response.Redirect(endp.RedirectUrl);
            }
        }
    }
}
