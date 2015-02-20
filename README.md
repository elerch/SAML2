# SAML2
Fork of SAML2 library on codeplex. This library removes dependencies on Asp.net

This project now consists of three libraries

1. SAML2.Core: This contains all the core logic from the original SAML2 library on codeplex and remains similar in style and structure. Configuration has been changed to no longer be married to System.Configuration. Filewatchers on metadata were problematic and have been removed on the belief that this additional functionality can be provided outside the core library
2. SAML2.AspNet: This contains all the ASP.Net bits from the original library including the configuration. This has not been tested, but theoretically SAML2.AspNet + Saml2.Core should be equivalent to the original single library on codeplex (minus the filewatchers)
3. Owin.Security.Saml: This contains an OWIN middleware implementation of SAMLP Service Provider. This library is the main driver for this effort.

Project Status
--------------

The project is currently usable for a Service Provider using redirect binding against a Shibboleth server and is likely usable for other SAMLP IdPs using Redirect binding. The included SelfHostOwinSPExample project provides a usable example against a live server at https://www.testshib.org. Other bindings can likely be added quickly (PRs welcome!).

There remains some cleanup to be done on the Owin side (e.g. configuration) and a number of general warts in the core library (e.g. logging) that were brought over from the original.

Configuring Owin
----------------

            appBuilder.UseSamlAuthentication(new Owin.Security.Saml.SamlAuthenticationOptions
            {
                Configuration = config,                     // Saml2 Core configuration
                RedirectAfterLogin = "/my application URI", // Temporary, will auto-detect later. PRs welcome
            });
            
Configuring the Saml2 Core Library
----------------------------------

            var myconfig = new Saml2Configuration
            {
                ServiceProvider = new ServiceProvider
                {
                    SigningCertificate = new X509Certificate2(FileEmbeddedResource("cert.pfx"), "pass", MachineKeySet),
                    Server = "https://localhost:44333/myapp",
                    Id = "https://localhost:44333/myapp"       // EntityId used in SAMLP to identify this SP
                },
                AllowedAudienceUris = new List<Uri>(new[] { new Uri("https://localhost:44333/myapp") })
            };
            // The following URLs are based on the defaults used by the middleware above
            myconfig.ServiceProvider.Endpoints.AddRange(new[] {
                new ServiceProviderEndpoint(EndpointType.SignOn, "/myapp/saml2/login", "/core"),
                new ServiceProviderEndpoint(EndpointType.Logout, "/myapp/saml2/logout", "/core"),
                new ServiceProviderEndpoint(EndpointType.Metadata, "/myapp/saml2/metadata")
            });
            myconfig.IdentityProviders.AddByMetadata("IdPMetadataFile.xml");
            myconfig.IdentityProviders.First().OmitAssertionSignatureCheck = true;
            myconfig.LoggingFactoryType = "SAML2.Logging.DebugLoggerFactory";
            return myconfig;

