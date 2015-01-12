using Microsoft.Owin.Security;
using SAML2.Config;

namespace Owin.Security.Saml
{
    public class SamlAuthenticationOptions : AuthenticationOptions
    {
        public SamlAuthenticationOptions() : base("SAML2") { }
        public Saml2Configuration Configuration { get; set; }
        public SamlAuthenticationNotifications Notifications { get; set; }
    }
}
