using SAML2.Bindings;
using SAML2.Config;
using SAML2.Logging;
using SAML2.Schema.Core;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;
using SAML2.Specification;
using SAML2.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace SAML2.Protocol
{
    public class Utility
    {
        private static readonly IInternalLogger logger = LoggerProvider.LoggerFor(typeof(Utility));

        /// <summary>
        /// Expected responses if session support is not present
        /// </summary>
        private static readonly HashSet<string> expectedResponses = new HashSet<string>();

        /// <summary>
        /// Session key used to save the current message id with the purpose of preventing replay attacks
        /// </summary>
        private const string ExpectedInResponseToSessionKey = "ExpectedInResponseTo";

        /// <summary>
        /// Gets the trusted signers.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="identityProvider">The identity provider.</param>
        /// <returns>List of trusted certificate signers.</returns>
        public static IEnumerable<AsymmetricAlgorithm> GetTrustedSigners(ICollection<KeyDescriptor> keys, IdentityProvider identityProvider)
        {
            if (keys == null) {
                throw new ArgumentNullException("keys");
            }

            foreach (var clause in keys.SelectMany(k => k.KeyInfo.Items.AsEnumerable().Cast<KeyInfoClause>())) {
                // Check certificate specifications
                if (clause is KeyInfoX509Data) {
                    var cert = XmlSignatureUtils.GetCertificateFromKeyInfo((KeyInfoX509Data)clause);
                    if (!CertificateSatisfiesSpecifications(identityProvider, cert)) {
                        continue;
                    }
                }

                var key = XmlSignatureUtils.ExtractKey(clause);
                yield return key;
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
        /// Retrieves the name of the issuer from an XmlElement containing an assertion.
        /// </summary>
        /// <param name="assertion">An XmlElement containing an assertion</param>
        /// <returns>The identifier of the Issuer</returns>
        public static string GetIssuer(XmlElement assertion)
        {
            var result = string.Empty;
            var list = assertion.GetElementsByTagName("Issuer", Saml20Constants.Assertion);
            if (list.Count > 0) {
                var issuer = (XmlElement)list[0];
                result = issuer.InnerText;
            }

            return result;
        }

        /// <summary>
        /// Gets the assertion.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="isEncrypted">if set to <c>true</c> [is encrypted].</param>
        /// <returns>The assertion XML.</returns>
        public static XmlElement GetAssertion(XmlElement el, out bool isEncrypted)
        {
            logger.Debug(TraceMessages.AssertionParse);

            var encryptedList = el.GetElementsByTagName(EncryptedAssertion.ElementName, Saml20Constants.Assertion);
            if (encryptedList.Count == 1) {
                isEncrypted = true;
                var encryptedAssertion = (XmlElement)encryptedList[0];

                logger.DebugFormat(TraceMessages.EncryptedAssertionFound, encryptedAssertion.OuterXml);

                return encryptedAssertion;
            }

            var assertionList = el.GetElementsByTagName(Assertion.ElementName, Saml20Constants.Assertion);
            if (assertionList.Count == 1) {
                isEncrypted = false;
                var assertion = (XmlElement)assertionList[0];

                logger.DebugFormat(TraceMessages.AssertionFound, assertion.OuterXml);

                return assertion;
            }

            logger.Warn(ErrorMessages.AssertionNotFound);

            isEncrypted = false;
            return null;
        }

        public static void AddExpectedResponseId(string id)
        {
            expectedResponses.Add(id);
        }


        /// <summary>
        /// Is called before the assertion is made into a strongly typed representation
        /// </summary>
        /// <param name="elem">The assertion element.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// 
        public static void PreHandleAssertion(XmlElement elem, IdentityProvider endpoint)
        {
            logger.DebugFormat(TraceMessages.AssertionPrehandlerCalled);

            if (endpoint != null && endpoint.Endpoints.DefaultLogoutEndpoint != null && !string.IsNullOrEmpty(endpoint.Endpoints.DefaultLogoutEndpoint.TokenAccessor)) {
                var idpTokenAccessor = Activator.CreateInstance(Type.GetType(endpoint.Endpoints.DefaultLogoutEndpoint.TokenAccessor, false)) as ISaml20IdpTokenAccessor;
                if (idpTokenAccessor != null) {
                    logger.DebugFormat("{0}.{1} called", idpTokenAccessor.GetType(), "ReadToken");
                    idpTokenAccessor.ReadToken(elem);
                    logger.DebugFormat("{0}.{1} finished", idpTokenAccessor.GetType(), "ReadToken");
                }
            }
        }

        /// <summary>
        /// Gets the decoded SAML response.
        /// </summary>
        /// <param name="samlResponse">This is base64 encoded SAML Response (usually SAMLResponse on query string)</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The decoded SAML response XML.</returns>
        public static XmlDocument GetDecodedSamlResponse(string samlResponse, Encoding encoding)
        {
            logger.Debug(TraceMessages.SamlResponseDecoding);


            var doc = new XmlDocument { PreserveWhitespace = true };
            samlResponse = encoding.GetString(Convert.FromBase64String(samlResponse));
            doc.LoadXml(samlResponse);

            logger.DebugFormat(TraceMessages.SamlResponseDecoded, samlResponse);

            return doc;
        }

        /// <summary>
        /// Gets the decrypted assertion.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>The decrypted <see cref="Saml20EncryptedAssertion"/>.</returns>
        public static Saml20EncryptedAssertion GetDecryptedAssertion(XmlElement elem, Saml2Configuration config)
        {
            logger.Debug(TraceMessages.EncryptedAssertionDecrypting);

            var decryptedAssertion = new Saml20EncryptedAssertion((RSA)config.ServiceProvider.SigningCertificate.PrivateKey);
            decryptedAssertion.LoadXml(elem);
            decryptedAssertion.Decrypt();

            logger.DebugFormat(TraceMessages.EncryptedAssertionDecrypted, decryptedAssertion.Assertion.DocumentElement.OuterXml);

            return decryptedAssertion;
        }

        /// <summary>
        /// Gets the status element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="Status" /> element.</returns>
        public static Status GetStatusElement(XmlElement element)
        {
            var statElem = element.GetElementsByTagName(Status.ElementName, Saml20Constants.Protocol)[0];
            return Serialization.DeserializeFromXmlString<Status>(statElem.OuterXml);
        }

        /// <summary>
        /// Checks for replay attack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        public static void CheckReplayAttack(XmlElement element, bool requireInResponseTo, IDictionary<string, object> session)
        {
            logger.Debug(TraceMessages.ReplayAttackCheck);

            var inResponseToAttribute = element.Attributes["InResponseTo"];
            if (!requireInResponseTo && inResponseToAttribute == null) {
                return;
            }
            if (inResponseToAttribute == null) {
                throw new Saml20Exception(ErrorMessages.ResponseMissingInResponseToAttribute);
            }

            var inResponseTo = inResponseToAttribute.Value;
            if (string.IsNullOrEmpty(inResponseTo)) {
                throw new Saml20Exception(ErrorMessages.ExpectedInResponseToEmpty);
            }

            if (session != null) {
                if (!session.ContainsKey(ExpectedInResponseToSessionKey)) {
                    throw new Saml20Exception(ErrorMessages.ExpectedInResponseToMissing);
                }
                var expectedInResponseTo = (string)session[ExpectedInResponseToSessionKey];

                if (inResponseTo != expectedInResponseTo) {
                    logger.ErrorFormat(ErrorMessages.ReplayAttack, inResponseTo, expectedInResponseTo);
                    throw new Saml20Exception(string.Format(ErrorMessages.ReplayAttack, inResponseTo, expectedInResponseTo));
                }
            } else {
                if (!expectedResponses.Contains(inResponseTo)) {
                    throw new Saml20Exception(ErrorMessages.ExpectedInResponseToMissing);
                }
                expectedResponses.Remove(inResponseTo);
            }
            logger.Debug(TraceMessages.ReplaceAttackCheckCleared);
        }

        public static void AddExpectedResponse(Saml20AuthnRequest request, IDictionary<string, object> session)
        {
            // Save request message id to session
            if (session != null) {
                session.Add(ExpectedInResponseToSessionKey, request.Id);
            } else {
                expectedResponses.Add(request.Id);
            }
        }

        /// <summary>
        /// Deserializes an assertion, verifies its signature and logs in the user if the assertion is valid.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="elem">The elem.</param>
        public static Saml20Assertion HandleAssertion(XmlElement elem, Saml2Configuration config, Func<string, object> getFromCache, Action<string, object, DateTime> setInCache)
        {
            logger.DebugFormat(TraceMessages.AssertionProcessing, elem.OuterXml);

            var issuer = GetIssuer(elem);
            var endp = IdpSelectionUtil.RetrieveIDPConfiguration(issuer, config);

            PreHandleAssertion(elem, endp);

            if (endp == null || endp.Metadata == null) {
                logger.Error(ErrorMessages.AssertionIdentityProviderUnknown);
                throw new Saml20Exception(ErrorMessages.AssertionIdentityProviderUnknown);
            }

            var quirksMode = endp.QuirksMode;
            var assertion = new Saml20Assertion(elem, null, quirksMode, config);

            // Check signatures
            if (!endp.OmitAssertionSignatureCheck) {
                if (!assertion.CheckSignature(GetTrustedSigners(endp.Metadata.GetKeys(KeyTypes.Signing), endp))) {
                    logger.Error(ErrorMessages.AssertionSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.AssertionSignatureInvalid);
                }
            }

            // Check expiration
            if (assertion.IsExpired) {
                logger.Error(ErrorMessages.AssertionExpired);
                throw new Saml20Exception(ErrorMessages.AssertionExpired);
            }

            // Check one time use
            if (assertion.IsOneTimeUse) {
                if (getFromCache(assertion.Id) != null) {
                    logger.Error(ErrorMessages.AssertionOneTimeUseExceeded);
                    throw new Saml20Exception(ErrorMessages.AssertionOneTimeUseExceeded);
                }

                setInCache(assertion.Id, string.Empty, assertion.NotOnOrAfter);
            }

            logger.DebugFormat(TraceMessages.AssertionParsed, assertion.Id);
            return assertion;
        }

        /// <summary>
        /// Decrypts an encrypted assertion, and sends the result to the HandleAssertion method.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="elem">The elem.</param>
        public static Saml20Assertion HandleEncryptedAssertion(XmlElement elem, Saml2Configuration config, Func<string, object> getFromCache, Action<string, object, DateTime> setInCache)
        {
            return HandleAssertion(GetDecryptedAssertion(elem, config).Assertion.DocumentElement, config, getFromCache, setInCache);
        }

        /// <summary>
        /// Handles the SOAP.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="inputStream">The input stream.</param>
        public static void HandleSoap(HttpArtifactBindingBuilder builder, Stream inputStream, Saml2Configuration config, Action<Saml20Assertion> signonCallback, Func<string, object> getFromCache, Action<string, object, DateTime> setInCache, IDictionary<string, object> session)
        {
            var parser = new HttpArtifactBindingParser(inputStream);
            logger.DebugFormat(TraceMessages.SOAPMessageParse, parser.SamlMessage.OuterXml);

            if (parser.IsArtifactResolve) {
                logger.Debug(TraceMessages.ArtifactResolveReceived);

                var idp = IdpSelectionUtil.RetrieveIDPConfiguration(parser.Issuer, config);
                if (!parser.CheckSamlMessageSignature(idp.Metadata.Keys)) {
                    logger.Error(ErrorMessages.ArtifactResolveSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ArtifactResolveSignatureInvalid);
                }

                builder.RespondToArtifactResolve(parser.ArtifactResolve, ((XmlDocument)getFromCache(parser.ArtifactResolve.Artifact)).DocumentElement);
            } else if (parser.IsArtifactResponse) {
                logger.Debug(TraceMessages.ArtifactResolveReceived);

                var idp = IdpSelectionUtil.RetrieveIDPConfiguration(parser.Issuer, config);
                if (!parser.CheckSamlMessageSignature(idp.Metadata.Keys)) {
                    logger.Error(ErrorMessages.ArtifactResponseSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ArtifactResponseSignatureInvalid);
                }

                var status = parser.ArtifactResponse.Status;
                if (status.StatusCode.Value != Saml20Constants.StatusCodes.Success) {
                    logger.ErrorFormat(ErrorMessages.ArtifactResponseStatusCodeInvalid, status.StatusCode.Value);
                    throw new Saml20Exception(string.Format(ErrorMessages.ArtifactResponseStatusCodeInvalid, status.StatusCode.Value));
                }

                if (parser.ArtifactResponse.Any.LocalName == Response.ElementName) {
                    Utility.CheckReplayAttack(parser.ArtifactResponse.Any, true, session);

                    var responseStatus = Utility.GetStatusElement(parser.ArtifactResponse.Any);
                    if (responseStatus.StatusCode.Value != Saml20Constants.StatusCodes.Success) {
                        logger.ErrorFormat(ErrorMessages.ArtifactResponseStatusCodeInvalid, responseStatus.StatusCode.Value);
                        throw new Saml20Exception(string.Format(ErrorMessages.ArtifactResponseStatusCodeInvalid, responseStatus.StatusCode.Value));
                    }

                    bool isEncrypted;
                    var assertion = Utility.GetAssertion(parser.ArtifactResponse.Any, out isEncrypted);
                    if (assertion == null) {
                        logger.Error(ErrorMessages.ArtifactResponseMissingAssertion);
                        throw new Saml20Exception(ErrorMessages.ArtifactResponseMissingAssertion);
                    }

                    var samlAssertion = isEncrypted
                        ? Utility.HandleEncryptedAssertion(assertion, config, getFromCache, setInCache)
                        : Utility.HandleAssertion(assertion, config, getFromCache, setInCache);
                    signonCallback(samlAssertion);
                } else {
                    logger.ErrorFormat(ErrorMessages.ArtifactResponseMissingResponse);
                    throw new Saml20Exception(ErrorMessages.ArtifactResponseMissingResponse);
                }
            } else {
                logger.ErrorFormat(ErrorMessages.SOAPMessageUnsupportedSamlMessage);
                throw new Saml20Exception(ErrorMessages.SOAPMessageUnsupportedSamlMessage);
            }
        }


        /// <summary>
        /// Handle the authentication response from the IDP.
        /// </summary>
        /// <param name="context">The context.</param>
        public static Saml20Assertion HandleResponse(Saml2Configuration config, string samlResponse, IDictionary<string, object> session, Func<string, object> getFromCache, Action<string, object, DateTime> setInCache)
        {
            var defaultEncoding = Encoding.UTF8;
            var doc = Utility.GetDecodedSamlResponse(samlResponse, defaultEncoding);
            logger.DebugFormat(TraceMessages.SamlResponseReceived, doc.OuterXml);

            // Determine whether the assertion should be decrypted before being validated.
            bool isEncrypted;
            var assertion = Utility.GetAssertion(doc.DocumentElement, out isEncrypted);
            if (isEncrypted) {
                assertion = Utility.GetDecryptedAssertion(assertion, config).Assertion.DocumentElement;
            }

            // Check if an encoding-override exists for the IdP endpoint in question
            var issuer = Utility.GetIssuer(assertion);
            var endpoint = IdpSelectionUtil.RetrieveIDPConfiguration(issuer, config);
            if (!endpoint.AllowReplayAttacks) {
                Utility.CheckReplayAttack(doc.DocumentElement, !endpoint.AllowIdPInitiatedSso, session);
            }
            var status = Utility.GetStatusElement(doc.DocumentElement);
            if (status.StatusCode.Value != Saml20Constants.StatusCodes.Success) {
                if (status.StatusCode.Value == Saml20Constants.StatusCodes.NoPassive) {
                    logger.Error(ErrorMessages.ResponseStatusIsNoPassive);
                    throw new Saml20Exception(ErrorMessages.ResponseStatusIsNoPassive);
                }

                logger.ErrorFormat(ErrorMessages.ResponseStatusNotSuccessful, status);
                throw new Saml20Exception(string.Format(ErrorMessages.ResponseStatusNotSuccessful, status));
            }

            if (!string.IsNullOrEmpty(endpoint.ResponseEncoding)) {
                Encoding encodingOverride;
                try {
                    encodingOverride = Encoding.GetEncoding(endpoint.ResponseEncoding);
                }
                catch (ArgumentException ex) {
                    logger.ErrorFormat(ErrorMessages.UnknownEncoding, endpoint.ResponseEncoding);
                    throw new ArgumentException(string.Format(ErrorMessages.UnknownEncoding, endpoint.ResponseEncoding), ex);
                }

                if (encodingOverride.CodePage != defaultEncoding.CodePage) {
                    var doc1 = GetDecodedSamlResponse(samlResponse, encodingOverride);
                    assertion = GetAssertion(doc1.DocumentElement, out isEncrypted);
                }
            }

            return HandleAssertion(assertion, config, getFromCache, setInCache);
        }

    }
}
