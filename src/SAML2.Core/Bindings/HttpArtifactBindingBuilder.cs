﻿using System;
using System.IO;
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
        private readonly Saml2Configuration config;
        private readonly Action<string> redirect;
        private readonly Action<string> sendResponseMessage;


	    /// <summary>
	    /// Initializes a new instance of the <see cref="HttpArtifactBindingBuilder"/> class.
	    /// </summary>
	    /// <param name="config"></param>
	    /// <param name="redirect">Action to perform when redirecting. Parameter will be destination URL</param>
	    /// <param name="sendResponseMessage">Action to send messages to response stream</param>
	    public HttpArtifactBindingBuilder(Saml2Configuration config, Action<string> redirect, Action<string> sendResponseMessage)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (redirect == null) throw new ArgumentNullException("redirect");
            if (sendResponseMessage == null) throw new ArgumentNullException("sendResponseMessage"); 
            this.redirect = redirect;
            this.config = config;
            this.sendResponseMessage = sendResponseMessage;
        }

	    /// <summary>
	    /// Creates an artifact and redirects the user to the IdP
	    /// </summary>
	    /// <param name="destination">The destination of the request.</param>
	    /// <param name="request">The authentication request.</param>
	    /// <param name="relayState">Relay state from client. May be null</param>
	    /// <param name="cacheInsert"></param>
	    public void RedirectFromLogin(IdentityProviderEndpoint destination, Saml20AuthnRequest request, string relayState, Action<string, object> cacheInsert)
        {
            var index = (short)config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.Index;
            var doc = request.GetXml();
            XmlSignatureUtils.SignDocument(doc, request.Request.Id, config.ServiceProvider.SigningCertificate);
            ArtifactRedirect(destination, index, doc, relayState, cacheInsert);
        }


	    /// <summary>
	    /// Creates an artifact for the LogoutRequest and redirects the user to the IdP.
	    /// </summary>
	    /// <param name="destination">The destination of the request.</param>
	    /// <param name="request">The logout request.</param>
	    /// <param name="relayState">The query string relay state value (relayState) to add to the communication</param>
	    /// <param name="cacheInsert"></param>
	    public void RedirectFromLogout(IdentityProviderEndpoint destination, Saml20LogoutRequest request, string relayState, Action<string, object> cacheInsert)
        {
            var index = (short)config.ServiceProvider.Endpoints.DefaultLogoutEndpoint.Index;
            var doc = request.GetXml();
            XmlSignatureUtils.SignDocument(doc, request.Request.Id, config.ServiceProvider.SigningCertificate);
            ArtifactRedirect(destination, index, doc, relayState, cacheInsert);
        }

	    /// <summary>
	    /// Creates an artifact for the LogoutResponse and redirects the user to the IdP.
	    /// </summary>
	    /// <param name="destination">The destination of the response.</param>
	    /// <param name="response">The logout response.</param>
	    /// <param name="relayState">The query string relay state value to add to the communication</param>
	    /// <param name="cacheInsert"></param>
	    public void RedirectFromLogout(IdentityProviderEndpoint destination, Saml20LogoutResponse response, string relayState, Action<string, object> cacheInsert)
        {
            var index = (short)config.ServiceProvider.Endpoints.DefaultLogoutEndpoint.Index;
            var doc = response.GetXml();
            XmlSignatureUtils.SignDocument(doc, response.Response.ID, config.ServiceProvider.SigningCertificate);

            ArtifactRedirect(destination, index, doc, relayState, cacheInsert);
        }

	    /// <summary>
	    /// Resolves an artifact.
	    /// </summary>
	    /// <returns>A stream containing the artifact response from the IdP</returns>
	    /// <param name="artifact">artifact from request ("SAMLart")</param>
	    /// <param name="relayState"></param>
	    /// <param name="config"></param>
	    public Stream ResolveArtifact(string artifact, string relayState, Saml2Configuration config)
        {
            var idpEndPoint = DetermineIdp(artifact);
            if (idpEndPoint == null)
            {
                throw new InvalidOperationException(ErrorMessages.ArtifactResolveIdentityProviderUnknown);
            }

            var endpointIndex = ArtifactUtil.GetEndpointIndex(artifact);
            var endpointUrl = idpEndPoint.Metadata.GetIDPARSEndpoint(endpointIndex);

            Logger.DebugFormat(TraceMessages.ArtifactResolveForKnownIdentityProvider, artifact, idpEndPoint.Id, endpointUrl);
            
            var resolve = Saml20ArtifactResolve.GetDefault(config.ServiceProvider.Id);
            resolve.Artifact = artifact;

            var doc = resolve.GetXml();
            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            XmlSignatureUtils.SignDocument(doc, resolve.Id, config.ServiceProvider.SigningCertificate);

            var artifactResolveString = doc.OuterXml;

            Logger.DebugFormat(TraceMessages.ArtifactResolved, artifactResolveString);

            return GetResponse(endpointUrl, artifactResolveString, idpEndPoint.ArtifactResolution, relayState);
        }

	    /// <summary>
	    /// Handles responses to an artifact resolve message.
	    /// </summary>
	    /// <param name="artifactResolve">The artifact resolve message.</param>
	    /// <param name="samlDoc"></param>
	    public void RespondToArtifactResolve(ArtifactResolve artifactResolve, XmlElement samlDoc)
        {
            var response = Saml20ArtifactResponse.GetDefault(config.ServiceProvider.Id);
            response.StatusCode = Saml20Constants.StatusCodes.Success;
            response.InResponseTo = artifactResolve.Id;
            response.SamlElement = samlDoc; //samlDoc.DocumentElement;

            var responseDoc = response.GetXml();
            if (responseDoc.FirstChild is XmlDeclaration)
            {
                responseDoc.RemoveChild(responseDoc.FirstChild);
            }

            XmlSignatureUtils.SignDocument(responseDoc, response.Id, config.ServiceProvider.SigningCertificate);

            Logger.DebugFormat(TraceMessages.ArtifactResolveResponseSent, artifactResolve.Artifact, responseDoc.OuterXml);

            sendResponseMessage(responseDoc.OuterXml);
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
	    /// <param name="cacheInsert"></param>
	    private void ArtifactRedirect(IdentityProviderEndpoint destination, short localEndpointIndex, XmlDocument signedSamlMessage, string relayState, Action<string, object> cacheInsert)
        {
            Logger.DebugFormat(TraceMessages.ArtifactRedirectReceived, signedSamlMessage.OuterXml);

            var sourceId = config.ServiceProvider.Id;
            var sourceIdHash = ArtifactUtil.GenerateSourceIdHash(sourceId);
            var messageHandle = ArtifactUtil.GenerateMessageHandle();

            var artifact = ArtifactUtil.CreateArtifact(HttpArtifactBindingConstants.ArtifactTypeCode, localEndpointIndex, sourceIdHash, messageHandle);
            cacheInsert(artifact, signedSamlMessage);

			var destinationUrl = string.Format( "{0}{1}{2}{3}{4}", destination.Url, ( destination.Url.EndsWith( "?" ) ? "&" : "?" ), HttpArtifactBindingConstants.ArtifactQueryStringName, "=", Uri.EscapeDataString( artifact ) );
            if (!string.IsNullOrEmpty(relayState))
            {
                destinationUrl += "&relayState=" + relayState;
            }

            Logger.DebugFormat(TraceMessages.ArtifactCreated, artifact);

            redirect(destinationUrl);
        }

        /// <summary>
        /// Determines which IdP an artifact has been sent from.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <returns>An IdP configuration element</returns>
        private IdentityProvider DetermineIdp(string artifact)
        {
            short typeCodeValue = -1;
            short endPointIndex = -1;
            var sourceIdHash = new byte[20];
            var messageHandle = new byte[20];

            if (ArtifactUtil.TryParseArtifact(artifact, ref typeCodeValue, ref endPointIndex, ref sourceIdHash, ref messageHandle))
            {
                foreach (IdentityProvider ep in config.IdentityProviders)
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
