using System;
using System.Linq;
using Owin;
using SAML2.Config;
using System.IO;

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
            var myconfig = new Saml2Configuration
            {
                ServiceProvider = new ServiceProvider
                {
                    SigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(FileEmbeddedResource("SelfHostOwinSPExample.sts_dev_certificate.pfx"), "test1234"),
                    Server = "https://localhost:44333/core",
                    Id = "https://localhost:44333/core",
                }
            };
            myconfig.ServiceProvider.Endpoints.Add(new ServiceProviderEndpoint
            {
                Type = EndpointType.SignOn,
                LocalPath = "/core/login",
                RedirectUrl = "/core"
            });
            myconfig.ServiceProvider.Endpoints.Add(new ServiceProviderEndpoint
            {
                Type = EndpointType.Logout,
                LocalPath = "/core/logout",
                RedirectUrl = "/core"
            });
            myconfig.ServiceProvider.Endpoints.Add(new ServiceProviderEndpoint
            {
                Type = EndpointType.Metadata,
                LocalPath = "/core/metadata"
            });
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