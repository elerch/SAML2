using System;
using System.Linq;
using Owin;
using SAML2.Config;
using System.IO;

namespace SelfHostOwinSPExample
{
    internal partial class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = GetSamlConfiguration();
#if TEST
            config = TestEnvironmentConfiguration();
#endif

            appBuilder.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = "SAML2",
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active
            });
            appBuilder.UseSamlAuthentication(new Owin.Security.Saml.SamlAuthenticationOptions
            {
                Configuration = config,
                RedirectAfterLogin = "/core",
            });
            appBuilder.Run(async c => {
                if (c.Authentication.User != null &&
                    c.Authentication.User.Identity != null &&
                    c.Authentication.User.Identity.IsAuthenticated) {
                    await c.Response.WriteAsync(c.Authentication.User.Identity.Name + "\r\n");
                    await c.Response.WriteAsync(c.Authentication.User.Identity.AuthenticationType + "\r\n");
                    foreach (var claim in c.Authentication.User.Identities.SelectMany(i => i.Claims))
                        await c.Response.WriteAsync(claim.Value + "\r\n");
                    await c.Response.WriteAsync("authenticated");
                } else {
                    // trigger authentication
                    c.Authentication.Challenge(c.Authentication.GetAuthenticationTypes().Select(d => d.AuthenticationType).ToArray());
                }
                return;
            });
        }

        private Saml2Configuration GetSamlConfiguration()
        {
            var myconfig = new Saml2Configuration
            {
                ServiceProvider = new ServiceProvider
                {
                    SigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(FileEmbeddedResource("SelfHostOwinSPExample.sts_dev_certificate.pfx"), "test1234", System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet),
                    Server = "https://localhost:44333/core",
                    Id = "https://localhost:44333/core"
                },
                AllowedAudienceUris = new System.Collections.Generic.List<Uri>(new[] { new Uri("https://localhost:44333/core") })
            };
            myconfig.ServiceProvider.Endpoints.AddRange(new[] {
                new ServiceProviderEndpoint(EndpointType.SignOn, "/core/saml2/login", "/core"),
                new ServiceProviderEndpoint(EndpointType.Logout, "/core/saml2/logout", "/core"),
                new ServiceProviderEndpoint(EndpointType.Metadata, "/core/saml2/metadata")
            });
            myconfig.IdentityProviders.AddByMetadataDirectory("..\\..\\Metadata");
            //myconfig.IdentityProviders.AddByMetadataUrl(new Uri("https://tas.fhict.nl/identity/saml2/metadata"));
            myconfig.IdentityProviders.First().OmitAssertionSignatureCheck = true;
            myconfig.LoggingFactoryType = "SAML2.Logging.DebugLoggerFactory";
            return myconfig;
        }

        private byte[] FileEmbeddedResource(string path)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = path;

            byte[] result = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var memoryStream = new MemoryStream()) {
                stream.CopyTo(memoryStream);
                result = memoryStream.ToArray();
            }
            return result;
        }
    }
}