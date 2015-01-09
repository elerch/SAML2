using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Bindings
{
    /// <summary>
    /// Implementation of the artifact over HTTP SOAP binding.
    /// </summary>
    public class HttpArtifactBindingBuilder : HttpSoapBindingBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpArtifactBindingBuilder"/> class.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public HttpArtifactBindingBuilder(HttpContext context) : base(context) { }

        /// <summary>
        /// Creates an artifact and redirects the user to the IdP
        /// </summary>
        /// <param name="destination">The destination of the request.</param>
        /// <param name="request">The authentication request.</param>
        public void RedirectFromLogin(IdentityProviderEndpointElement destination, Saml20AuthnRequest request)
        {
            var config = Saml2Config.GetConfig();
            var index = (short)config.ServiceProvider.Endpoints.SignOnEndpoint.Index;
            var doc = request.GetXml();
            XmlSignatureUtils.SignDocument(doc, request.Request.Id);
            ArtifactRedirect(destination, index, doc, Context.Request.Params["relayState"]);
        }

        /// <summary>
        /// Creates an artifact for the LogoutRequest and redirects the user to the IdP.
        /// </summary>
        /// <param name="destination">The destination of the request.</param>
        /// <param name="request">The logout request.</param>
        public void RedirectFromLogout(IdentityProviderEndpointElement destination, Saml20LogoutRequest request)
        {
            RedirectFromLogout(destination, request, Context.Request.Params["relayState"]);
        }

        /// <summary>
        /// Creates an artifact for the LogoutRequest and redirects the user to the IdP.
        /// </summary>
        /// <param name="destination">The destination of the request.</param>
        /// <param name="request">The logout request.</param>
        /// <param name="relayState">The query string relay state value to add to the communication</param>
        public void RedirectFromLogout(IdentityProviderEndpointElement destination, Saml20LogoutRequest request, string relayState)
        {
            var config = Saml2Config.GetConfig();
            var index = (short)config.ServiceProvider.Endpoints.LogoutEndpoint.Index;
            var doc = request.GetXml();
            XmlSignatureUtils.SignDocument(doc, request.Request.Id);
            ArtifactRedirect(destination, index, doc, relayState);
        }

        /// <summary>
        /// Creates an artifact for the LogoutResponse and redirects the user to the IdP.
        /// </summary>
        /// <param name="destination">The destination of the response.</param>
        /// <param name="response">The logout response.</param>
        public void RedirectFromLogout(IdentityProviderEndpointElement destination, Saml20LogoutResponse response)
        {
            var config = Saml2Config.GetConfig();
            var index = (short)config.ServiceProvider.Endpoints.LogoutEndpoint.Index;
            var doc = response.GetXml();
            XmlSignatureUtils.SignDocument(doc, response.Response.ID);

            ArtifactRedirect(destination, index, doc, Context.Request.Params["relayState"]);
        }

        /// <summary>
        /// Resolves an artifact.
        /// </summary>
        /// <returns>A stream containing the artifact response from the IdP</returns>
        public Stream ResolveArtifact()
        {
            var artifact = Context.Request.Params["SAMLart"];
            var idpEndPoint = DetermineIdp(artifact);
            if (idpEndPoint == null)
            {
                throw new InvalidOperationException(ErrorMessages.ArtifactResolveIdentityProviderUnknown);
            }

            var endpointIndex = ArtifactUtil.GetEndpointIndex(artifact);
            var endpointUrl = idpEndPoint.Metadata.GetIDPARSEndpoint(endpointIndex);

            Logger.DebugFormat(TraceMessages.ArtifactResolveForKnownIdentityProvider, artifact, idpEndPoint.Id, endpointUrl);
            var resolve = Saml20ArtifactResolve.GetDefault();
            resolve.Artifact = artifact;

            var doc = resolve.GetXml();
            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            XmlSignatureUtils.SignDocument(doc, resolve.Id);

            var artifactResolveString = doc.OuterXml;

            Logger.DebugFormat(TraceMessages.ArtifactResolved, artifactResolveString);

            return GetResponse(endpointUrl, artifactResolveString, idpEndPoint.ArtifactResolution);
        }

        /// <summary>
        /// Handles responses to an artifact resolve message.
        /// </summary>
        /// <param name="artifactResolve">The artifact resolve message.</param>
        public void RespondToArtifactResolve(ArtifactResolve artifactResolve)
        {
            var samlDoc = (XmlDocument)Context.Cache.Get(artifactResolve.Artifact);
            
            var response = Saml20ArtifactResponse.GetDefault();
            response.StatusCode = Saml20Constants.StatusCodes.Success;
            response.InResponseTo = artifactResolve.Id;
            response.SamlElement = samlDoc.DocumentElement;

            var responseDoc = response.GetXml();
            if (responseDoc.FirstChild is XmlDeclaration)
            {
                responseDoc.RemoveChild(responseDoc.FirstChild);
            }

            XmlSignatureUtils.SignDocument(responseDoc, response.Id);

            Logger.DebugFormat(TraceMessages.ArtifactResolveResponseSent, artifactResolve.Artifact, responseDoc.OuterXml);

            SendResponseMessage(responseDoc.OuterXml);
        }

        /// <summary>
        /// Determines if the contents of 2 byte arrays are identical
        /// </summary>
        /// <param name="a">The first array</param>
        /// <param name="b">The second array</param>
        /// <returns>True of the byte arrays are equal, else false.</returns>
        private static bool ByteArraysAreEqual(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Handles all artifact creations and redirects.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="localEndpointIndex">Index of the local endpoint.</param>
        /// <param name="signedSamlMessage">The signed SAML message.</param>
        /// <param name="relayState">The query string relay state value to add to the communication</param>
        private void ArtifactRedirect(IdentityProviderEndpointElement destination, short localEndpointIndex, XmlDocument signedSamlMessage, string relayState)
        {
            Logger.DebugFormat(TraceMessages.ArtifactRedirectReceived, signedSamlMessage.OuterXml);

            var config = Saml2Config.GetConfig();
            var sourceId = config.ServiceProvider.Id;
            var sourceIdHash = ArtifactUtil.GenerateSourceIdHash(sourceId);
            var messageHandle = ArtifactUtil.GenerateMessageHandle();

            var artifact = ArtifactUtil.CreateArtifact(HttpArtifactBindingConstants.ArtifactTypeCode, localEndpointIndex, sourceIdHash, messageHandle);
            Context.Cache.Insert(artifact, signedSamlMessage, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration);

            var destinationUrl = destination.Url + "?" + HttpArtifactBindingConstants.ArtifactQueryStringName + "=" + HttpUtility.UrlEncode(artifact);
            if (!string.IsNullOrEmpty(relayState))
            {
                destinationUrl += "&relayState=" + relayState;
            }

            Logger.DebugFormat(TraceMessages.ArtifactCreated, artifact);

            Context.Response.Redirect(destinationUrl);
        }

        /// <summary>
        /// Determines which IdP an artifact has been sent from.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <returns>An IdP configuration element</returns>
        private IdentityProviderElement DetermineIdp(string artifact)
        {
            var config = Saml2Config.GetConfig();
            
            short typeCodeValue = -1;
            short endPointIndex = -1;
            var sourceIdHash = new byte[20];
            var messageHandle = new byte[20];

            if (ArtifactUtil.TryParseArtifact(artifact, ref typeCodeValue, ref endPointIndex, ref sourceIdHash, ref messageHandle))
            {
                foreach (IdentityProviderElement ep in config.IdentityProviders)
                {
                    var hash = ArtifactUtil.GenerateSourceIdHash(ep.Id);
                    if (ByteArraysAreEqual(sourceIdHash, hash))
                    {
                        return ep;
                    }
                }
            }
            
            return null;
        }
    }
}
