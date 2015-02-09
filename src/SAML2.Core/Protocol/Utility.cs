using SAML2.Config;
using SAML2.Logging;
using SAML2.Schema.Core;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;
using SAML2.Specification;
using SAML2.Utils;
using System;
using System.Collections.Generic;
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
        private static readonly IInternalLogger Logger = LoggerProvider.LoggerFor(typeof(Utility));

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
            Logger.Debug(TraceMessages.AssertionParse);

            var encryptedList = el.GetElementsByTagName(EncryptedAssertion.ElementName, Saml20Constants.Assertion);
            if (encryptedList.Count == 1) {
                isEncrypted = true;
                var encryptedAssertion = (XmlElement)encryptedList[0];

                Logger.DebugFormat(TraceMessages.EncryptedAssertionFound, encryptedAssertion.OuterXml);

                return encryptedAssertion;
            }

            var assertionList = el.GetElementsByTagName(Assertion.ElementName, Saml20Constants.Assertion);
            if (assertionList.Count == 1) {
                isEncrypted = false;
                var assertion = (XmlElement)assertionList[0];

                Logger.DebugFormat(TraceMessages.AssertionFound, assertion.OuterXml);

                return assertion;
            }

            Logger.Warn(ErrorMessages.AssertionNotFound);

            isEncrypted = false;
            return null;
        }

        /// <summary>
        /// Is called before the assertion is made into a strongly typed representation
        /// </summary>
        /// <param name="elem">The assertion element.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// 
        public static void PreHandleAssertion(XmlElement elem, IdentityProvider endpoint)
        {
            Logger.DebugFormat(TraceMessages.AssertionPrehandlerCalled);

            if (endpoint != null && endpoint.Endpoints.DefaultLogoutEndpoint != null && !string.IsNullOrEmpty(endpoint.Endpoints.DefaultLogoutEndpoint.TokenAccessor)) {
                var idpTokenAccessor = Activator.CreateInstance(Type.GetType(endpoint.Endpoints.DefaultLogoutEndpoint.TokenAccessor, false)) as ISaml20IdpTokenAccessor;
                if (idpTokenAccessor != null) {
                    Logger.DebugFormat("{0}.{1} called", idpTokenAccessor.GetType(), "ReadToken");
                    idpTokenAccessor.ReadToken(elem);
                    Logger.DebugFormat("{0}.{1} finished", idpTokenAccessor.GetType(), "ReadToken");
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
            Logger.Debug(TraceMessages.SamlResponseDecoding);


            var doc = new XmlDocument { PreserveWhitespace = true };
            samlResponse = encoding.GetString(Convert.FromBase64String(samlResponse));
            doc.LoadXml(samlResponse);

            Logger.DebugFormat(TraceMessages.SamlResponseDecoded, samlResponse);

            return doc;
        }

        /// <summary>
        /// Gets the decrypted assertion.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>The decrypted <see cref="Saml20EncryptedAssertion"/>.</returns>
        public static Saml20EncryptedAssertion GetDecryptedAssertion(XmlElement elem, Saml2Configuration config)
        {
            Logger.Debug(TraceMessages.EncryptedAssertionDecrypting);

            var decryptedAssertion = new Saml20EncryptedAssertion((RSA)config.ServiceProvider.SigningCertificate.PrivateKey);
            decryptedAssertion.LoadXml(elem);
            decryptedAssertion.Decrypt();

            Logger.DebugFormat(TraceMessages.EncryptedAssertionDecrypted, decryptedAssertion.Assertion.DocumentElement.OuterXml);

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
    }
}
