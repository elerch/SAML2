using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin;

namespace Owin.Security.Saml
{
    /// <summary>
    /// OWIN middleware for obtaining identities using Saml protocol.
    /// </summary>
    public class SamlAuthenticationMiddleware : AuthenticationMiddleware<SamlAuthenticationOptions>
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a <see cref="SamlAuthenticationMiddleware"/>
        /// </summary>
        /// <param name="next">The next middleware in the OWIN pipeline to invoke</param>
        /// <param name="app">The OWIN application</param>
        /// <param name="options">Configuration options for the middleware</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "A reference is maintained.")]
        public SamlAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, SamlAuthenticationOptions options)
            : base(next, options)
        {
            _logger = app.CreateLogger<SamlAuthenticationMiddleware>();

            if (Options.Notifications == null)
            {
                Options.Notifications = new SamlAuthenticationNotifications();
            }


            if (Options.Configuration == null)
            {
                throw new ArgumentOutOfRangeException("options", "Configuration must be set prior to using SamlAuthenticationMiddleware");
            }
        }

        /// <summary>
        /// Provides the <see cref="AuthenticationHandler"/> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="AuthenticationHandler"/> configured with the <see cref="SamlAuthenticationOptions"/> supplied to the constructor.</returns>
        protected override AuthenticationHandler<SamlAuthenticationOptions> CreateHandler()
        {
            return new SamlAuthenticationHandler(_logger);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Managed by caller")]
        private static HttpMessageHandler ResolveHttpMessageHandler(SamlAuthenticationOptions options)
        {
            HttpMessageHandler handler = new WebRequestHandler();
            return handler;
        }
    }
}