using Microsoft.Owin.Security;
using SAML2.Config;

namespace Owin.Security.Saml
{
    public class SamlAuthenticationOptions : AuthenticationOptions
    {
        public Saml2Configuration Configuration { get; set; }
        public SamlAuthenticationNotifications Notifications { get; set; }
    }
}
