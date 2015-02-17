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

            return app.Use<SamlAuthenticationMiddleware>(app, options);
        }
    }
}