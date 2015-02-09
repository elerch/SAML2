using System;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using SAML2.Config;
using System.Runtime.Caching;

namespace Owin.Security.Saml
{
    public class SamlAuthenticationOptions : AuthenticationOptions
    {
        // TODO: IDisposable...
        private readonly MemoryCache memoryCache = new MemoryCache("samlCache");
        public SamlAuthenticationOptions() : base("SAML2") {
            Description = new AuthenticationDescription
            {
                AuthenticationType = "SAML2",
                Caption = "Saml 2.0 Authentication protocol for OWIN"
            };
            MetadataPath = "/saml2/metadata";
            LoginPath = "/saml2/login";
            LogoutPath = "/saml2/logout";
            GetFromCache = s => memoryCache.Get(s);
            SetInCache = (s, o, d) => memoryCache.Set(s, o, d);
        }
        public Saml2Configuration Configuration { get; set; }
        public Func<string, object> GetFromCache { get; set; }


        /// <summary>
        /// Defines login path for all bindings. Defaults to /saml2/login
        /// </summary>
        public string LoginPath { get; private set; }

        /// <summary>
        /// Defines logout path for all bindings. Defaults to /saml2/logout
        /// </summary>
        public string LogoutPath { get; private set; }
        /// <summary>
        /// Defines path used to acquire metadata. Defaults to /saml2/metadata
        /// </summary>
        public string MetadataPath { get; set; }
        public SamlAuthenticationNotifications Notifications { get; set; }

        /// <summary>
        /// Session state (handy for validating responses are as expected in multi-server environments). Optional, default null
        /// </summary>
        public IDictionary<string, object> Session { get; set; }
        public Action<string, object, DateTime> SetInCache { get; set; }
    }
}
