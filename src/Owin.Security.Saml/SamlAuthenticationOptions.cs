using Microsoft.Owin.Security;
using SAML2.Config;

namespace Owin.Security.Saml
{
    public class SamlAuthenticationOptions : AuthenticationOptions
    {
        public SamlAuthenticationOptions() : base("SAML2") {
            Description = new AuthenticationDescription
            {
                AuthenticationType = "SAML2",
                Caption = "Saml 2.0 Authentication protocol for OWIN"
            };
            MetadataPath = "/saml2/metadata";
        }
        public Saml2Configuration Configuration { get; set; }
        public string MetadataPath { get; set; }
        public SamlAuthenticationNotifications Notifications { get; set; }
    }
}
