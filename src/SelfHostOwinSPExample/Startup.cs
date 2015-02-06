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
            appBuilder.UseSamlAuthentication(new Owin.Security.Saml.SamlAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                Configuration = config
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
            var myconfig = new Saml2Configuration
            {
                ServiceProvider = new ServiceProvider
                {
                    SigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(FileEmbeddedResource("SelfHostOwinSPExample.sts_dev_certificate.pfx"), "test1234", System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet),
                    Server = "https://localhost:44333/core",
                    Id = "https://localhost:44333/core"
                },
            };
            myconfig.ServiceProvider.Endpoints.AddRange(new[] {
                new ServiceProviderEndpoint(EndpointType.SignOn, "/core/login", "/core", BindingType.Redirect),
                new ServiceProviderEndpoint(EndpointType.Logout, "/core/logout", "/core", BindingType.Redirect),
                new ServiceProviderEndpoint(EndpointType.Metadata, "/core/metadata")
            });
            myconfig.IdentityProviders.AddByMetadataDirectory("..\\..\\Metadata");
            myconfig.LoggingFactoryType = "SAML2.Logging.DebugLoggerFactory";
            SAML2.Logging.LoggerProvider.Configuration = myconfig;
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