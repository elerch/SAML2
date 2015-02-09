using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using SAML2.Config;
using SAML2.Bindings;
using SAML2.AspNet;
using System.IO;

namespace SAML2.Protocol
{
    /// <summary>
    /// Base class for all SAML20 specific endpoints.
    /// </summary>
    public abstract class Saml20AbstractEndpointHandler : AbstractEndpointHandler
    {
        /// <summary>
        /// Parameter name for IDP choice
        /// </summary>
        public const string IdpChoiceParameterName = "cidp";

        /// <summary>
        /// Key used to override <c>ForceAuthn</c> setting
        /// </summary>
        public const string IdpForceAuthn = "IDPForceAuthn";

        /// <summary>
        /// Key used to override IsPassive setting
        /// </summary>
        public const string IdpIsPassive = "IDPIsPassive";

        /// <summary>
        /// Key used to save login session
        /// </summary>
        public const string IdpLoginSessionKey = "LoginIDPId";

        /// <summary>
        /// Key used to save the IDP name id in session context
        /// </summary>
        public const string IdpNameId = "IDPNameId";

        /// <summary>
        /// Used to save the name id format of the assertion
        /// </summary>
        public const string IdpNameIdFormat = "IDPNameIdFormat";

        /// <summary>
        /// Key used to save SessionId
        /// </summary>
        public const string IdpSessionIdKey = "IDPSessionID";

        /// <summary>
        /// Key used to save temporary session id
        /// </summary>
        public const string IdpTempSessionKey = "TempIDPId";

        /// <summary>
        /// Gets or sets a value indicating whether configuration has been validated
        /// </summary>
        public static bool Validated { get; set; }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public sealed override void ProcessRequest(HttpContext context)
        {
            try
            {
                CheckConfiguration();
                Handle(context);
            }
            catch (ThreadAbortException)
            {
                // This will swallow the ThreadAbortException automatically thrown by Response.Redirect, Response.End, and Server.Transfer
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Abstract handler function
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected abstract void Handle(HttpContext ctx);

        /// <summary>
        /// Checks the configuration elements and redirects to an error page if something is missing or wrong.
        /// </summary>
        private void CheckConfiguration()
        {
            if (Validated)
            {
                return;
            }

            Validated = BindingUtility.ValidateConfiguration(ConfigurationFactory.Instance.Configuration);
        }

        protected static HttpArtifactBindingBuilder GetBuilder(HttpContext context)
        {
            return new HttpArtifactBindingBuilder(
                ConfigurationFactory.Instance.Configuration,
                context.Response.Redirect,
                m => SendResponseMessage(m, context));
        }

        protected static void SendResponseMessage(string message, HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            using (var writer = new StreamWriter(context.Response.OutputStream)) {
                writer.Write(HttpSoapBindingBuilder.WrapInSoapEnvelope(message));
                writer.Flush();
                writer.Close();
            }
            context.Response.End();
        }
    }
}
