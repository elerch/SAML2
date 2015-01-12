using System;
using Microsoft.Owin.Security;
using SAML2.Config;
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
        /// <param name="wtrealm">The application identifier.</param>
        /// <param name="metadataAddress">The address to retrieve the Saml metadata from.</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseSamlAuthentication(this IAppBuilder app, string wtrealm, string metadataAddress)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (string.IsNullOrEmpty(wtrealm))
            {
                throw new ArgumentNullException("wtrealm");
            }
            if (string.IsNullOrEmpty(metadataAddress))
            {
                throw new ArgumentNullException("metadataAddress");
            }

            return app.UseSamlAuthentication(new SamlAuthenticationOptions()
            {
                Wtrealm = wtrealm,
                MetadataAddress = metadataAddress,
            });
        }

        /// <summary>
        /// Adds the <see cref="SamlAuthenticationMiddleware"/> into the OWIN runtime.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="SamlOptions">SamlAuthenticationOptions configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseSamlAuthentication(this IAppBuilder app, IConfigurationProvider configurationReader)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (configurationReader == null) throw new ArgumentNullException("configurationReader"); 

            return app.Use<SamlAuthenticationMiddleware>(app, configurationReader);
        }
    }
}