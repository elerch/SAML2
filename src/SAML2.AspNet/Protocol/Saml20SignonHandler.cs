using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using SAML2.Bindings;
using SAML2.Config;
using SAML2.Protocol.Pages;
using SAML2.Schema.Core;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;
using SAML2.Specification;
using SAML2.Utils;
using SAML2.AspNet;

namespace SAML2.Protocol
{
    /// <summary>
    /// Implements a SAML 2.0 protocol sign-on endpoint. Handles all SAML bindings.
    /// </summary>
    public class Saml20SignonHandler : Saml20AbstractEndpointHandler
    {
        /// <summary>
        /// Expected responses if session support is not present
        /// </summary>
        internal static HashSet<string> ExpectedResponses { get; private set; }

        static Saml20SignonHandler()
        {
            ExpectedResponses = new HashSet<string>();
        }
        /// <summary>
        /// Session key used to save the current message id with the purpose of preventing replay attacks
        /// </summary>
        private const string ExpectedInResponseToSessionKey = "ExpectedInResponseTo";

        /// <summary>
        /// The certificate for the endpoint.
        /// </summary>
        private readonly X509Certificate2 _certificate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20SignonHandler"/> class.
        /// </summary>
        public Saml20SignonHandler() : this(null)
        { }

        public Saml20SignonHandler(Saml2Configuration config)
        {
            _certificate = config.ServiceProvider.SigningCertificate;

            // Read the proper redirect url from config
            try {
                RedirectUrl = config.ServiceProvider.Endpoints.DefaultSignOnEndpoint.RedirectUrl;
            }
            catch (Exception e) {
                Logger.Error(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the trusted signers.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="identityProvider">The identity provider.</param>
        /// <returns>List of trusted certificate signers.</returns>
        public static IEnumerable<AsymmetricAlgorithm> GetTrustedSigners(ICollection<KeyDescriptor> keys, IdentityProvider identityProvider)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            var result = new List<AsymmetricAlgorithm>(keys.Count);
            foreach (var keyDescriptor in keys)
            {
                foreach (KeyInfoClause clause in (KeyInfo)keyDescriptor.KeyInfo)
                {
                    // Check certificate specifications
                    if (clause is KeyInfoX509Data)
                    {
                        var cert = XmlSignatureUtils.GetCertificateFromKeyInfo((KeyInfoX509Data)clause);
                        if (!CertificateSatisfiesSpecifications(identityProvider, cert))
                        {
                            continue;
                        }
                    }

                    var key = XmlSignatureUtils.ExtractKey(clause);
                    result.Add(key);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the assertion.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="isEncrypted">if set to <c>true</c> [is encrypted].</param>
        /// <returns>The assertion XML.</returns>
        internal static XmlElement GetAssertion(XmlElement el, out bool isEncrypted)
        {
            Logger.Debug(TraceMessages.AssertionParse);

            var encryptedList = el.GetElementsByTagName(EncryptedAssertion.ElementName, Saml20Constants.Assertion);
            if (encryptedList.Count == 1)
            {
                isEncrypted = true;
                var encryptedAssertion = (XmlElement)encryptedList[0];

                Logger.DebugFormat(TraceMessages.EncryptedAssertionFound, encryptedAssertion.OuterXml);

                return encryptedAssertion;
            }

            var assertionList = el.GetElementsByTagName(Assertion.ElementName, Saml20Constants.Assertion);
            if (assertionList.Count == 1)
            {
                isEncrypted = false;
                var assertion = (XmlElement)assertionList[0];

                Logger.DebugFormat(TraceMessages.AssertionFound, assertion.OuterXml);

                return assertion;
            }

            Logger.Warn(ErrorMessages.AssertionNotFound);

            isEncrypted = false;
            return null;
        }
        
        protected override void Handle(HttpContext context)
        {
            Handle(context, ConfigurationFactory.Instance.Configuration);
        }
        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Handle(HttpContext context, Saml2Configuration config)
        {
            Logger.Debug(TraceMessages.SignOnHandlerCalled);

            // Some IdP's are known to fail to set an actual value in the SOAPAction header
            // so we just check for the existence of the header field.
            if (Array.Exists(context.Request.Headers.AllKeys, s => s == SoapConstants.SoapAction))
            {
                HandleSoap(context, context.Request.InputStream, config);
                return;
            }

            if (!string.IsNullOrEmpty(context.Request.Params["SAMLart"]))
            {
                HandleArtifact(context, config);
            }

            if (!string.IsNullOrEmpty(context.Request.Params["SamlResponse"]))
            {
                HandleResponse(context, config);
            }
            else
            {
                if (config.CommonDomainCookie.Enabled && context.Request.QueryString["r"] == null
                    && context.Request.Params["cidp"] == null)
                {
                    Logger.Debug(TraceMessages.CommonDomainCookieRedirectForDiscovery);
                    context.Response.Redirect(config.CommonDomainCookie.LocalReaderEndpoint);
                }
                else
                {
                    Logger.WarnFormat(ErrorMessages.UnauthenticatedAccess, context.Request.RawUrl);
                    SendRequest(context, config);
                }
            }
        }

        /// <summary>
        /// Is called before the assertion is made into a strongly typed representation
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <param name="elem">The assertion element.</param>
        /// <param name="endpoint">The endpoint.</param>
        protected virtual void PreHandleAssertion(HttpContext context, XmlElement elem, IdentityProvider endpoint)
        {
            Logger.DebugFormat(TraceMessages.AssertionPrehandlerCalled);

            if (endpoint != null && endpoint.Endpoints.DefaultLogoutEndpoint != null && !string.IsNullOrEmpty(endpoint.Endpoints.DefaultLogoutEndpoint.TokenAccessor))
            {
                var idpTokenAccessor = Activator.CreateInstance(Type.GetType(endpoint.Endpoints.DefaultLogoutEndpoint.TokenAccessor, false)) as ISaml20IdpTokenAccessor;
                if (idpTokenAccessor != null)
                {
                    Logger.DebugFormat("{0}.{1} called", idpTokenAccessor.GetType(), "ReadToken");
                    idpTokenAccessor.ReadToken(elem);
                    Logger.DebugFormat("{0}.{1} finished", idpTokenAccessor.GetType(), "ReadToken");
                }
            }
        }

        /// <summary>
        /// Determines whether the certificate is satisfied by all specifications.
        /// </summary>
        /// <param name="idp">The identity provider.</param>
        /// <param name="cert">The cert.</param>
        /// <returns><c>true</c> if certificate is satisfied by all specifications; otherwise, <c>false</c>.</returns>
        private static bool CertificateSatisfiesSpecifications(IdentityProvider idp, X509Certificate2 cert)
        {
            return SpecificationFactory.GetCertificateSpecifications(idp).All(spec => spec.IsSatisfiedBy(cert));
        }

        /// <summary>
        /// Checks for replay attack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        private static void CheckReplayAttack(HttpContext context, XmlElement element, bool requireInResponseTo)
        {
            Logger.Debug(TraceMessages.ReplayAttackCheck);

            var inResponseToAttribute = element.Attributes["InResponseTo"];
            if (!requireInResponseTo && inResponseToAttribute == null) 
            {
                return;
            }
            if (inResponseToAttribute == null)
            {
                throw new Saml20Exception(ErrorMessages.ResponseMissingInResponseToAttribute);
            }

            var inResponseTo = inResponseToAttribute.Value;
            if (string.IsNullOrEmpty(inResponseTo))
            {
                throw new Saml20Exception(ErrorMessages.ExpectedInResponseToEmpty);
            }

            if (context.Session != null) {
                var expectedInResponseTo = (string)context.Session[ExpectedInResponseToSessionKey];
                if (string.IsNullOrEmpty(expectedInResponseTo)) 
                {
                    throw new Saml20Exception(ErrorMessages.ExpectedInResponseToMissing);
                }

                if (inResponseTo != expectedInResponseTo) 
                {
                    Logger.ErrorFormat(ErrorMessages.ReplayAttack, inResponseTo, expectedInResponseTo);
                    throw new Saml20Exception(string.Format(ErrorMessages.ReplayAttack, inResponseTo, expectedInResponseTo));
                }
            } 
            else
            {
                if (!ExpectedResponses.Contains(inResponseTo)) 
                {
                    throw new Saml20Exception(ErrorMessages.ExpectedInResponseToMissing);
                }
                ExpectedResponses.Remove(inResponseTo);
            }
            Logger.Debug(TraceMessages.ReplaceAttackCheckCleared);
        }

        /// <summary>
        /// Gets the decoded SAML response.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The decoded SAML response XML.</returns>
        private static XmlDocument GetDecodedSamlResponse(HttpContext context, Encoding encoding)
        {
            Logger.Debug(TraceMessages.SamlResponseDecoding);

            var base64 = context.Request.Params["SAMLResponse"];

            var doc = new XmlDocument { PreserveWhitespace = true };
            var samlResponse = encoding.GetString(Convert.FromBase64String(base64));
            doc.LoadXml(samlResponse);

            Logger.DebugFormat(TraceMessages.SamlResponseDecoded, samlResponse);

            return doc;
        }

        /// <summary>
        /// Gets the decrypted assertion.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>The decrypted <see cref="Saml20EncryptedAssertion"/>.</returns>
        private static Saml20EncryptedAssertion GetDecryptedAssertion(XmlElement elem, Saml2Configuration config)
        {
            Logger.Debug(TraceMessages.EncryptedAssertionDecrypting);

            var decryptedAssertion = new Saml20EncryptedAssertion((RSA)config.ServiceProvider.SigningCertificate.PrivateKey);
            decryptedAssertion.LoadXml(elem);
            decryptedAssertion.Decrypt();

            Logger.DebugFormat(TraceMessages.EncryptedAssertionDecrypted, decryptedAssertion.Assertion.DocumentElement.OuterXml);

            return decryptedAssertion;
        }

        /// <summary>
        /// Retrieves the name of the issuer from an XmlElement containing an assertion.
        /// </summary>
        /// <param name="assertion">An XmlElement containing an assertion</param>
        /// <returns>The identifier of the Issuer</returns>
        private static string GetIssuer(XmlElement assertion)
        {
            var result = string.Empty;
            var list = assertion.GetElementsByTagName("Issuer", Saml20Constants.Assertion);
            if (list.Count > 0)
            {
                var issuer = (XmlElement)list[0];
                result = issuer.InnerText;
            }

            return result;
        }

        /// <summary>
        /// Gets the status element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="Status" /> element.</returns>
        private static Status GetStatusElement(XmlElement element)
        {
            var statElem = element.GetElementsByTagName(Status.ElementName, Saml20Constants.Protocol)[0];
            return Serialization.DeserializeFromXmlString<Status>(statElem.OuterXml);
        }

        /// <summary>
        /// Handles executing the login.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assertion">The assertion.</param>
        private void DoSignOn(HttpContext context, Saml20Assertion assertion, Saml2Configuration config)
        {
            // User is now logged in at IDP specified in tmp
            context.Items[IdpLoginSessionKey] = context.Session != null ? context.Session[IdpTempSessionKey] : context.Items[IdpTempSessionKey];
            context.Items[IdpSessionIdKey] = assertion.SessionIndex;
            context.Items[IdpNameIdFormat] = assertion.Subject.Format;
            context.Items[IdpNameId] = assertion.Subject.Value;

            Logger.DebugFormat(TraceMessages.SignOnProcessed, assertion.SessionIndex, assertion.Subject.Value, assertion.Subject.Format);

            Logger.Debug(TraceMessages.SignOnActionsExecuting);
            // TODO: Signon event
            //foreach (var action in Actions.Actions.GetActions(config))
            //{
            //    Logger.DebugFormat("{0}.{1} called", action.GetType(), "LoginAction()");

            //    action.SignOnAction(this, context, assertion, config);

            //    Logger.DebugFormat("{0}.{1} finished", action.GetType(), "LoginAction()");
            //}
        }

        /// <summary>
        /// Handles the artifact.
        /// </summary>
        /// <param name="context">The context.</param>
        private void HandleArtifact(HttpContext context, Saml2Configuration config)
        {
            var builder = new HttpArtifactBindingBuilder(context, config);
            var inputStream = builder.ResolveArtifact();
            
            HandleSoap(context, inputStream, config);
        }

        /// <summary>
        /// Deserializes an assertion, verifies its signature and logs in the user if the assertion is valid.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="elem">The elem.</param>
        private void HandleAssertion(HttpContext context, XmlElement elem, Saml2Configuration config)
        {
            Logger.DebugFormat(TraceMessages.AssertionProcessing, elem.OuterXml);

            var issuer = GetIssuer(elem);
            var endp = RetrieveIDPConfiguration(issuer, config);

            PreHandleAssertion(context, elem, endp);

            if (endp == null || endp.Metadata == null)
            {
                Logger.Error(ErrorMessages.AssertionIdentityProviderUnknown);
                throw new Saml20Exception(ErrorMessages.AssertionIdentityProviderUnknown);
            }

            var quirksMode = endp.QuirksMode;
            var assertion = new Saml20Assertion(elem, null, quirksMode, config);

            // Check signatures
            if (!endp.OmitAssertionSignatureCheck)
            {
                if (!assertion.CheckSignature(GetTrustedSigners(endp.Metadata.GetKeys(KeyTypes.Signing), endp)))
                {
                    Logger.Error(ErrorMessages.AssertionSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.AssertionSignatureInvalid);
                }
            }

            // Check expiration
            if (assertion.IsExpired)
            {
                Logger.Error(ErrorMessages.AssertionExpired);
                throw new Saml20Exception(ErrorMessages.AssertionExpired);
            }

            // Check one time use
            if (assertion.IsOneTimeUse)
            {
                if (context.Cache[assertion.Id] != null)
                {
                    Logger.Error(ErrorMessages.AssertionOneTimeUseExceeded);
                    throw new Saml20Exception(ErrorMessages.AssertionOneTimeUseExceeded);
                }

                context.Cache.Insert(assertion.Id, string.Empty, null, assertion.NotOnOrAfter, Cache.NoSlidingExpiration);
            }

            Logger.DebugFormat(TraceMessages.AssertionParsed, assertion.Id);

            DoSignOn(context, assertion, config);
        }

        /// <summary>
        /// Decrypts an encrypted assertion, and sends the result to the HandleAssertion method.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="elem">The elem.</param>
        private void HandleEncryptedAssertion(HttpContext context, XmlElement elem, Saml2Configuration config)
        {
            HandleAssertion(context, GetDecryptedAssertion(elem, config).Assertion.DocumentElement, config);
        }

        /// <summary>
        /// Handle the authentication response from the IDP.
        /// </summary>
        /// <param name="context">The context.</param>
        private void HandleResponse(HttpContext context, Saml2Configuration config)
        {
            var defaultEncoding = Encoding.UTF8;
            var doc = GetDecodedSamlResponse(context, defaultEncoding);
            Logger.DebugFormat(TraceMessages.SamlResponseReceived, doc.OuterXml);

            // Determine whether the assertion should be decrypted before being validated.
            bool isEncrypted;
            var assertion = GetAssertion(doc.DocumentElement, out isEncrypted);
            if (isEncrypted) 
            {
                assertion = GetDecryptedAssertion(assertion, config).Assertion.DocumentElement;
            }

            // Check if an encoding-override exists for the IdP endpoint in question
            var issuer = GetIssuer(assertion);
            var endpoint = RetrieveIDPConfiguration(issuer, config);
            if (!endpoint.AllowReplayAttacks) 
            {
                CheckReplayAttack(context, doc.DocumentElement, !endpoint.AllowIdPInitiatedSso);
            }
            var status = GetStatusElement(doc.DocumentElement);
            if (status.StatusCode.Value != Saml20Constants.StatusCodes.Success)
            {
                if (status.StatusCode.Value == Saml20Constants.StatusCodes.NoPassive)
                {
                    Logger.Error(ErrorMessages.ResponseStatusIsNoPassive);
                    throw new Saml20Exception(ErrorMessages.ResponseStatusIsNoPassive);
                }

                Logger.ErrorFormat(ErrorMessages.ResponseStatusNotSuccessful, status);
                throw new Saml20Exception(string.Format(ErrorMessages.ResponseStatusNotSuccessful, status));
            }

            if (!string.IsNullOrEmpty(endpoint.ResponseEncoding))
            {
                Encoding encodingOverride;
                try
                {
                    encodingOverride = Encoding.GetEncoding(endpoint.ResponseEncoding);
                }
                catch (ArgumentException ex)
                {
                    Logger.ErrorFormat(ErrorMessages.UnknownEncoding, endpoint.ResponseEncoding);
                    throw new ArgumentException(string.Format(ErrorMessages.UnknownEncoding, endpoint.ResponseEncoding), ex);
                }

                if (encodingOverride.CodePage != defaultEncoding.CodePage)
                {
                    var doc1 = GetDecodedSamlResponse(context, encodingOverride);
                    assertion = GetAssertion(doc1.DocumentElement, out isEncrypted);
                }
            }

            HandleAssertion(context, assertion, config);
        }

        /// <summary>
        /// Handles the SOAP.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="inputStream">The input stream.</param>
        private void HandleSoap(HttpContext context, Stream inputStream, Saml2Configuration config)
        {
            var parser = new HttpArtifactBindingParser(inputStream);
            Logger.DebugFormat(TraceMessages.SOAPMessageParse, parser.SamlMessage.OuterXml);

            var builder = new HttpArtifactBindingBuilder(context, config);

            if (parser.IsArtifactResolve)
            {
                Logger.Debug(TraceMessages.ArtifactResolveReceived);

                var idp = RetrieveIDPConfiguration(parser.Issuer);
                if (!parser.CheckSamlMessageSignature(idp.Metadata.Keys))
                {
                    Logger.Error(ErrorMessages.ArtifactResolveSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ArtifactResolveSignatureInvalid);
                }

                builder.RespondToArtifactResolve(parser.ArtifactResolve);
            }
            else if (parser.IsArtifactResponse)
            {
                Logger.Debug(TraceMessages.ArtifactResolveReceived);

                var idp = RetrieveIDPConfiguration(parser.Issuer);
                if (!parser.CheckSamlMessageSignature(idp.Metadata.Keys))
                {
                    Logger.Error(ErrorMessages.ArtifactResponseSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ArtifactResponseSignatureInvalid);
                }

                var status = parser.ArtifactResponse.Status;
                if (status.StatusCode.Value != Saml20Constants.StatusCodes.Success)
                {
                    Logger.ErrorFormat(ErrorMessages.ArtifactResponseStatusCodeInvalid, status.StatusCode.Value);
                    throw new Saml20Exception(string.Format(ErrorMessages.ArtifactResponseStatusCodeInvalid, status.StatusCode.Value));
                }

                if (parser.ArtifactResponse.Any.LocalName == Response.ElementName)
                {
                    CheckReplayAttack(context, parser.ArtifactResponse.Any, true);

                    var responseStatus = GetStatusElement(parser.ArtifactResponse.Any);
                    if (responseStatus.StatusCode.Value != Saml20Constants.StatusCodes.Success)
                    {
                        Logger.ErrorFormat(ErrorMessages.ArtifactResponseStatusCodeInvalid, responseStatus.StatusCode.Value);
                        throw new Saml20Exception(string.Format(ErrorMessages.ArtifactResponseStatusCodeInvalid, responseStatus.StatusCode.Value));
                    }

                    bool isEncrypted;
                    var assertion = GetAssertion(parser.ArtifactResponse.Any, out isEncrypted);
                    if (assertion == null)
                    {
                        Logger.Error(ErrorMessages.ArtifactResponseMissingAssertion);
                        throw new Saml20Exception(ErrorMessages.ArtifactResponseMissingAssertion);
                    }

                    if (isEncrypted)
                    {
                        HandleEncryptedAssertion(context, assertion, config);
                    }
                    else
                    {
                        HandleAssertion(context, assertion, config);
                    }
                }
                else
                {
                    Logger.ErrorFormat(ErrorMessages.ArtifactResponseMissingResponse);
                    throw new Saml20Exception(ErrorMessages.ArtifactResponseMissingResponse);
                }
            }
            else
            {
                Logger.ErrorFormat(ErrorMessages.SOAPMessageUnsupportedSamlMessage);
                throw new Saml20Exception(ErrorMessages.SOAPMessageUnsupportedSamlMessage);
            }
        }

        /// <summary>
        /// Send an authentication request to the IDP.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendRequest(HttpContext context, Saml2Configuration config)
        {
            // See if the "ReturnUrl" - parameter is set.
            var returnUrl = context.Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && context.Session != null)
            {
                context.Session["RedirectUrl"] = returnUrl;
            }            

            var idp = RetrieveIDP(context, config);
            if (idp == null)
            {
                // Display a page to the user where she can pick the IDP
                Logger.DebugFormat(TraceMessages.IdentityProviderRedirect);

                var page = new SelectSaml20IDP();
                page.ProcessRequest(context);
                return;
            }

            var authnRequest = Saml20AuthnRequest.GetDefault(config);
            TransferClient(idp, authnRequest, context, config);            
        }

        /// <summary>
        /// Transfers the client.
        /// </summary>
        /// <param name="identityProvider">The identity provider.</param>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        private void TransferClient(IdentityProvider identityProvider, Saml20AuthnRequest request, HttpContext context, Saml2Configuration config)
        {
            // Set the last IDP we attempted to login at.
            if (context.Session != null) 
            {
                context.Session[IdpTempSessionKey] = identityProvider.Id;
            }
            context.Items[IdpTempSessionKey] = identityProvider.Id;

            // Determine which endpoint to use from the configuration file or the endpoint metadata.
            var destination = DetermineEndpointConfiguration(BindingType.Redirect, identityProvider.Endpoints.DefaultSignOnEndpoint, identityProvider.Metadata.SSOEndpoints);
            request.Destination = destination.Url;

            if (identityProvider.ForceAuth)
            {
                request.ForceAuthn = true;
            }

            // Check isPassive status
            var isPassiveFlag = context.Session != null ? context.Session[IdpIsPassive] : null;
            if (isPassiveFlag != null && (bool)isPassiveFlag)
            {
                request.IsPassive = true;
                context.Session[IdpIsPassive] = null;
            }

            if (identityProvider.IsPassive)
            {
                request.IsPassive = true;
            }

            // Check if request should forceAuthn
            var forceAuthnFlag =  context.Session != null ? context.Session[IdpForceAuthn] : null;
            if (forceAuthnFlag != null && (bool)forceAuthnFlag)
            {
                request.ForceAuthn = true;
                context.Session[IdpForceAuthn] = null;
            }

            // Check if protocol binding should be forced
            if (identityProvider.Endpoints.DefaultSignOnEndpoint != null)
            {
                if (!string.IsNullOrEmpty(identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding))
                {
                    request.ProtocolBinding = identityProvider.Endpoints.DefaultSignOnEndpoint.ForceProtocolBinding;
                }
            }

            // Save request message id to session
            if (context.Session != null) 
            {
                context.Session.Add(ExpectedInResponseToSessionKey, request.Id);
            } 
            else 
            {
                ExpectedResponses.Add(request.Id);
            }

            switch (destination.Binding)
            {
                case BindingType.Redirect:
                    Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpRedirect);

                    var redirectBuilder = new HttpRedirectBindingBuilder
                                      {
                                          SigningKey = _certificate.PrivateKey,
                                          Request = request.GetXml().OuterXml
                                      };

                    Logger.DebugFormat(TraceMessages.AuthnRequestSent, redirectBuilder.Request);

                    var redirectLocation = request.Destination + "?" + redirectBuilder.ToQuery();
                    context.Response.Redirect(redirectLocation, true);
                    break;
                case BindingType.Post:
                    Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpPost);

                    var postBuilder = new HttpPostBindingBuilder(destination);

                    // Honor the ForceProtocolBinding and only set this if it's not already set
                    if (string.IsNullOrEmpty(request.ProtocolBinding))
                    {
                        request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpPost;
                    }

                    var requestXml = request.GetXml();
                    XmlSignatureUtils.SignDocument(requestXml, request.Id, config.ServiceProvider.SigningCertificate);
                    postBuilder.Request = requestXml.OuterXml;

                    Logger.DebugFormat(TraceMessages.AuthnRequestSent, postBuilder.Request);

                    context.Response.Write(postBuilder.GetPage());
                    break;
                case BindingType.Artifact:
                    Logger.DebugFormat(TraceMessages.AuthnRequestPrepared, identityProvider.Id, Saml20Constants.ProtocolBindings.HttpArtifact);

                    var artifactBuilder = new HttpArtifactBindingBuilder(context, config);

                    // Honor the ForceProtocolBinding and only set this if it's not already set
                    if (string.IsNullOrEmpty(request.ProtocolBinding))
                    {
                        request.ProtocolBinding = Saml20Constants.ProtocolBindings.HttpArtifact;
                    }

                    Logger.DebugFormat(TraceMessages.AuthnRequestSent, request.GetXml().OuterXml);

                    artifactBuilder.RedirectFromLogin(destination, request);
                    break;
                default:
                    Logger.Error(ErrorMessages.EndpointBindingInvalid);
                    throw new Saml20Exception(ErrorMessages.EndpointBindingInvalid);
            }
        }

    }
}
