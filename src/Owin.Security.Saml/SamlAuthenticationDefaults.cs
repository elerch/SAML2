using System.Diagnostics.CodeAnalysis;

namespace Owin.Security.Saml
{
    /// <summary>
    /// Default values related to Saml authentication middleware
    /// </summary>
    public static class SamlAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for SamlAuthenticationOptions.AuthenticationType
        /// </summary>
        public const string AuthenticationType = "Federation";

        /// <summary>
        /// The prefix used to provide a default SamlAuthenticationOptions.CookieName
        /// </summary>
        public const string CookiePrefix = "Saml.";

        /// <summary>
        /// The prefix used to provide a default SamlAuthenticationOptions.CookieName
        /// </summary>
        public const string CookieName = "SamlAuth";

        /// <summary>
        /// The default value for SamlAuthenticationOptions.Caption.
        /// </summary>
        public const string Caption = "Saml";

        internal const string WctxKey = "WsFedOwinState";
    }
}
