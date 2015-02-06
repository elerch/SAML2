using SAML2.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfHostOwinSPExample.Metadata_MtUAT
{
    internal partial class Startup
    {
        public Saml2Configuration TestEnvironmentConfiguration()
        {
            var myconfig = new Saml2Configuration
            {
                ServiceProvider = new ServiceProvider
                {
                    SigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("../../Metadata-Test/certificate.pfx", "", System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet),
                    Server = "http://localhost:7777/identity",
                    Id = "http://localhost:7777/identity"
                }
            };
            myconfig.ServiceProvider.Endpoints.AddRange(new[] {
                new ServiceProviderEndpoint(EndpointType.SignOn, "/identity/login", "/identity", BindingType.Redirect),
                new ServiceProviderEndpoint(EndpointType.Logout, "/identity/logout", "/identity", BindingType.Redirect),
                new ServiceProviderEndpoint(EndpointType.Metadata, "/identity/metadata")
            });
            myconfig.IdentityProviders.AddByMetadata("..\\..\\Metadata-Test\\uat.xml");
            SAML2.Logging.LoggerProvider.Configuration = myconfig;
            return myconfig;
        }
    }
}
