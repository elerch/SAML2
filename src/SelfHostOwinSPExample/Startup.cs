using System;
using System.Linq;
using Owin;
using SAML2.Config;

namespace SelfHostOwinSPExample
{
    internal class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseSamlAuthentication(new Owin.Security.Saml.SamlAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                Configuration = GetSamlConfiguration()
            });
            appBuilder.Run(c => {
                if (c.Authentication.User != null &&
                    c.Authentication.User.Identity != null &&
                    c.Authentication.User.Identity.IsAuthenticated) {
                    return c.Response.WriteAsync("hello world - authenticated");
                } else {
                    // trigger authentication
                    c.Authentication.Challenge(c.Authentication.GetAuthenticationTypes().Select(d => d.AuthenticationType).ToArray());
                    return System.Threading.Tasks.Task.Delay(0);
                }
            });
        }

        private Saml2Configuration GetSamlConfiguration()
        {
            var myconfig = new Saml2Configuration();
            SAML2.Logging.LoggerProvider.Configuration = myconfig;
            return myconfig;
        }
    }
}