using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using NUnit.Framework;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Utils;

namespace SAML2.Tests
{
    /// <summary>
    /// Utility class for generating assertions.
    /// </summary>
    public class AssertionUtil
    {
        /// <summary>
        /// The testing certificate.
        /// </summary>
        private static X509Certificate2 _cert;

        /// <summary>
        /// Returns the <c>Saml20Assertion</c> as an XmlDocument as used by the Assertion class.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>The XML document.</returns>
        public static XmlDocument ConvertAssertionToXml(Assertion assertion)
        {
            if (assertion == null)
            {
                throw new ArgumentNullException("assertion");
            }

            var res = new XmlDocument { PreserveWhitespace = true };
            res.Load(new StringReader(Serialization.SerializeToXmlString(assertion)));

            return res;
        }

        /// <summary>
        /// Loads an assertion, deserializes it using the <code>Assertion</code> class and returns the
        /// resulting <code>Assertion</code> instance.
        /// </summary>
        /// <param name="assertionFile">The assertion file.</param>
        /// <returns>The <see cref="Saml20Assertion"/>.</returns>
        public static Saml20Assertion DeserializeToken(string assertionFile)
        {
            return DeserializeToken(assertionFile, true);
        }

        /// <summary>
        /// Loads an assertion, deserializes it using the <code>Assertion</code> class and returns the
        /// resulting <code>Assertion</code> instance.
        /// </summary>
        /// <param name="assertionFile">The assertion file.</param>
        /// <param name="verify">if set to <c>true</c> [verify].</param>
        /// <returns>The <see cref="Saml20Assertion"/>.</returns>
        public static Saml20Assertion DeserializeToken(string assertionFile, bool verify)
        {
            var document = LoadXmlDocument(assertionFile);

            var assertion = new Saml20Assertion(document.DocumentElement, null, false);

            if (verify)
            {
                var result = new List<AsymmetricAlgorithm>(1);
                foreach (KeyInfoClause clause in assertion.GetSignatureKeys())
                {
                    var key = XmlSignatureUtils.ExtractKey(clause);
                    result.Add(key);
                }

                assertion.CheckValid(result);
            }

            return assertion;
        }

        /// <summary>
        /// Gets the audiences.
        /// </summary>
        /// <returns>The audience list used for tests.</returns>
        public static List<string> GetAudiences()
        {
            return new List<string>(new[] { "urn:borger.dk:id" });
        }

        /// <summary>
        /// Assembles our basic test assertion
        /// </summary>
        /// <returns>The <see cref="Assertion"/>.</returns>
        public static Assertion GetBasicAssertion()
        {
            var assertion = new Assertion
                                {
                                    Issuer = new NameId(),
                                    Id = "_b8977dc86cda41493fba68b32ae9291d",
                                    IssueInstant = DateTime.UtcNow,
                                    Version = "2.0"
                                };

            assertion.Issuer.Value = GetBasicIssuer();
            assertion.Subject = new Subject();
            var subjectConfirmation = new SubjectConfirmation
            {
                Method = SubjectConfirmation.BearerMethod,
                SubjectConfirmationData =
                    new SubjectConfirmationData
                    {
                        NotOnOrAfter = new DateTime(2008, 12, 31, 12, 0, 0, 0),
                        Recipient = "http://borger.dk"
                    }
            };
            assertion.Subject.Items = new object[] { subjectConfirmation };
            assertion.Conditions = new Conditions { NotOnOrAfter = new DateTime(2008, 12, 31, 12, 0, 0, 0) };
            var audienceRestriction = new AudienceRestriction { Audience = GetAudiences() };
            assertion.Conditions.Items = new List<ConditionAbstract>(new ConditionAbstract[] { audienceRestriction });

            AuthnStatement authnStatement;
            {
                authnStatement = new AuthnStatement();
                assertion.Items = new StatementAbstract[] { authnStatement };
                authnStatement.AuthnInstant = new DateTime(2008, 1, 8);
                authnStatement.SessionIndex = "70225885";
                authnStatement.AuthnContext = new AuthnContext
                                                  {
                                                      Items = new object[]
                                                                  {
                                                                      "urn:oasis:names:tc:SAML:2.0:ac:classes:X509",
                                                                      "http://www.safewhere.net/authncontext/declref"
                                                                  },
                                                      ItemsElementName = new[]
                                                                             {
                                                                                 AuthnContextType.AuthnContextClassRef,
                                                                                 AuthnContextType.AuthnContextDeclRef
                                                                             }
                                                  };
            }

            AttributeStatement attributeStatement;
            {
                attributeStatement = new AttributeStatement();
                var surName = new SamlAttribute
                    {
                        FriendlyName = "SurName",
                        Name = "urn:oid:2.5.4.4",
                        NameFormat = SamlAttribute.NameformatUri,
                        AttributeValue = new[] { "Fry" }
                    };

                var commonName = new SamlAttribute
                    {
                        FriendlyName = "CommonName",
                        Name = "urn:oid:2.5.4.3",
                        NameFormat = SamlAttribute.NameformatUri,
                        AttributeValue = new[] { "Philip J. Fry" }
                    };

                var userName = new SamlAttribute
                    {
                        Name = "urn:oid:0.9.2342.19200300.100.1.1",
                        NameFormat = SamlAttribute.NameformatUri,
                        AttributeValue = new[] { "fry" }
                    };

                var email = new SamlAttribute
                    {
                        FriendlyName = "Email",
                        Name = "urn:oid:0.9.2342.19200300.100.1.3",
                        NameFormat = SamlAttribute.NameformatUri,
                        AttributeValue = new[] { "fry@planetexpress.com.earth" }
                    };

                attributeStatement.Items = new object[] { surName, commonName, userName, email };
            }

            assertion.Items = new StatementAbstract[] { authnStatement, attributeStatement };

            return assertion;
        }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <returns>The issuer.</returns>
        public static string GetBasicIssuer()
        {
            return "urn:TokenService/Safewhere";
        }

        /// <summary>
        /// Retrieve our development certificate.
        /// </summary>
        /// <returns>The testing certificate.</returns>
        public static X509Certificate2 GetCertificate()
        {
            if (_cert == null)
            {
                _cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                Assert.That(_cert.HasPrivateKey, "Certificate no longer contains a private key. Modify test.");
            }

            return _cert;
        }

        /// <summary>
        /// Generates an unsigned assertion for use in the other tests.
        /// </summary>
        /// <returns>The XML document.</returns>
        public static XmlDocument GetTestAssertion()
        {
            var res = new XmlDocument { PreserveWhitespace = true };
            res.Load(new StringReader(Serialization.SerializeToXmlString(GetBasicAssertion())));

            return res;
        }

        /// <summary>
        /// Gets the trusted signers.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <returns>A list of trusted signing certificates.</returns>
        public static IEnumerable<AsymmetricAlgorithm> GetTrustedSigners(string issuer)
        {
            if (issuer == null)
            {
                throw new ArgumentNullException("issuer");
            }

            var config = Saml2Config.GetConfig();

            var idpEndpoint = config.IdentityProviders.FirstOrDefault(x => x.Id == issuer);
            if (idpEndpoint == null)
            {
                throw new InvalidOperationException(string.Format("No idp endpoint found for issuer {0}", issuer));
            }

            if (idpEndpoint.Metadata == null)
            {
                throw new InvalidOperationException(string.Format("No metadata found for issuer {0}", issuer));
            }

            if (idpEndpoint.Metadata.Keys == null)
            {
                throw new InvalidOperationException(string.Format("No key descriptors found in metadata found for issuer {0}", issuer));
            }

            var result = new List<AsymmetricAlgorithm>(1);
            foreach (var key in idpEndpoint.Metadata.Keys)
            {
                foreach (KeyInfoClause clause in (KeyInfo)key.KeyInfo)
                {
                    var aa = XmlSignatureUtils.ExtractKey(clause);
                    result.Add(aa);
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="assertionFile">The assertion file.</param>
        /// <returns>The XML document.</returns>
        public static XmlDocument LoadXmlDocument(string assertionFile)
        {
            using (var fs = File.OpenRead(assertionFile))
            {
                var document = new XmlDocument { PreserveWhitespace = true };
                document.Load(fs);
                fs.Close();

                return document;
            }
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="assertionFile">The assertion file.</param>
        /// <returns>The XML document.</returns>
        public static XmlDocument LoadBase64EncodedXmlDocument(string assertionFile)
        {
            var assertionBase64 = File.ReadAllText(@"Assertions\fobs-assertion2");
            var assertionBytes = Convert.FromBase64String(assertionBase64);

            var document = new XmlDocument { PreserveWhitespace = true };
            document.Load(new MemoryStream(assertionBytes));

            return document;
        }
    }
}
