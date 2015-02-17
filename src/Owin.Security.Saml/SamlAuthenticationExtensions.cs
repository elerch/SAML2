using System;
using Owin.Security.Saml;

namespace Owin
{
    /// <summary>
    /// Extension methods for using <see cref="SamlAuthenticationMiddleware"/>
    /// </summary>
    public static class SamlAuthenticationExtensions
    {
        /// <summary>
        /// Adds the <see cref="SamlAuthenticationMiddleware"/> into the OWIN runtime.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Saml2Configuration configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseSamlAuthentication(this IAppBuilder app, SamlAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null) throw new ArgumentNullException("options");

            SAML2.Logging.LoggerProvider.Configuration = SAML2.Logging.LoggerProvider.Configuration ?? options.Configuration;

            app.Map(options.MetadataPath, metadataapp => {
                metadataapp.Run(new SamlMetadataWriter(options.Configuration).WriteMetadataDocument);
            });

            
            // curl "https://localhost:44333/core/login" --2.0 -H "Host: localhost:44333" -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; rv:35.0) Gecko/20100101 Firefox/35.0" -H "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" -H "Accept-Language: en-US,en;q=0.5" --compressed -H "Referer: https://idp.testshib.org/idp/profile/SAML2/Redirect/SSO" -H "Connection: keep-alive" -H "Cache-Control: max-age=0" --data "SAMLResponse=PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz48c2FtbDJwOlJlc3BvbnNlIHhtbG5zOnNhbWwycD0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOnByb3RvY29sIiBEZXN0aW5hdGlvbj0iaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzMvY29yZS9sb2dpbiIgSUQ9Il80NWQzMmIzOGRmZmE1YjNjNDc3OThmYTIzODdhNGY1MCIgSW5SZXNwb25zZVRvPSJpZGM2MTI2MmEwZWQ4MTQyNThhYWFhMTdkZjEyNDJlYjI3IiBJc3N1ZUluc3RhbnQ9IjIwMTUtMDItMDdUMDA6NTY6MDEuNTM2WiIgVmVyc2lvbj0iMi4wIj48c2FtbDI6SXNzdWVyIHhtbG5zOnNhbWwyPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6YXNzZXJ0aW9uIiBGb3JtYXQ9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpuYW1laWQtZm9ybWF0OmVudGl0eSI"%"2BaHR0cHM6Ly9pZHAudGVzdHNoaWIub3JnL2lkcC9zaGliYm9sZXRoPC9zYW1sMjpJc3N1ZXI"%"2BPHNhbWwycDpTdGF0dXM"%"2BPHNhbWwycDpTdGF0dXNDb2RlIFZhbHVlPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6c3RhdHVzOlJlc3BvbmRlciI"%"2BPHNhbWwycDpTdGF0dXNDb2RlIFZhbHVlPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6c3RhdHVzOkludmFsaWROYW1lSURQb2xpY3kiLz48L3NhbWwycDpTdGF0dXNDb2RlPjxzYW1sMnA6U3RhdHVzTWVzc2FnZT5SZXF1aXJlZCBOYW1lSUQgZm9ybWF0IG5vdCBzdXBwb3J0ZWQ8L3NhbWwycDpTdGF0dXNNZXNzYWdlPjwvc2FtbDJwOlN0YXR1cz48L3NhbWwycDpSZXNwb25zZT4"%"3D"
            //app.Map(options.LoginPath, loginApp => {
            //    loginApp.Use(new SamlLoginHandler(options).Invoke);
            //});

            return app.Use<SamlAuthenticationMiddleware>(app, options);
        }
    }
}