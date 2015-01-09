using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using NUnit.Framework;

namespace SAML2.Tests
{
    /// <summary>
    /// Contains tests that verify the signatures of the sample assertions in the Assertions directory. 
    /// One tests performs a "bare-bone" verification, while another verifies using the <code>Assertion</code> class.
    /// </summary>
    [TestFixture]
    public class SignatureTest
    {
        #region Assertion verification

        /// <summary>
        /// Verifies the signature in the "<c>Saml2Assertion_01</c>" file. The assertion in the file is valid.
        /// </summary>
        [Test]
        public void VerifyValidSignaturesAreValid()
        {
            Assert.That(VerifySignature(@"Assertions\Saml2Assertion_01"));
            Assert.That(VerifySignature(@"Assertions\Saml2Assertion_02"));
            Assert.That(VerifySignature(@"Assertions\Saml2Assertion_03"));            
        }

        /// <summary>
        /// Verifies that SignedXml will detect assertions that have been tampered with.
        /// </summary>
        [Test]
        public void VerifyManipulatedSignatureAreInvalid()
        {
            Assert.IsFalse(VerifySignature(@"Assertions\EvilSaml2Assertion_01"));
            Assert.IsFalse(VerifySignature(@"Assertions\EvilSaml2Assertion_02"));
            Assert.IsFalse(VerifySignature(@"Assertions\EvilSaml2Assertion_03"));
        }

        /// <summary>
        /// Deserializes the test tokens using the Safewhere DK-SAML class.
        /// </summary>
        /// <remarks>
        /// TODO: test data needs fixing
        /// </remarks>
        [Ignore]
        public void TestSaml20TokenVerification01()
        {
            AssertionUtil.DeserializeToken(@"Assertions\Saml2Assertion_01");
            AssertionUtil.DeserializeToken(@"Assertions\Saml2Assertion_02");
            AssertionUtil.DeserializeToken(@"Assertions\Saml2Assertion_03");
        }

        /// <summary>
        /// Attempts to deserialize an invalid SAML-token. Tests that the Assertion class immediately "explodes".
        /// </summary>
        [Test]
        [ExpectedException(typeof(Saml20Exception), ExpectedMessage = "Signature could not be verified.")]
        public void TestSaml20TokenVerification02()
        {
            AssertionUtil.DeserializeToken(@"Assertions\EvilSaml2Assertion_01");
        }

        /// <summary>
        /// Attempts to deserialize an invalid SAML-token. Tests that the Assertion class immediately "explodes".
        /// </summary>
        [Test]
        [ExpectedException(typeof(Saml20Exception), ExpectedMessage = "Signature could not be verified.")]
        public void TestSaml20TokenVerification03()
        {
            AssertionUtil.DeserializeToken(@"Assertions\EvilSaml2Assertion_02");
        }

        /// <summary>
        /// Attempts to deserialize an invalid SAML-token. Tests that the Assertion class immediately "explodes".
        /// </summary>
        [Test]
        [ExpectedException(typeof(Saml20Exception), ExpectedMessage = "Signature could not be verified.")]
        public void TestSaml20TokenVerification04()
        {
            AssertionUtil.DeserializeToken(@"Assertions\EvilSaml2Assertion_03");
        }

        #endregion

        #region Assertion signing verification

        /// <summary>
        /// Tests the signing and verification of an assertion.
        /// </summary>
        [Test]
        public void AssertionCanBeSignedAndVerified()
        {
            // Arrange
            var token = AssertionUtil.GetTestAssertion();
            SignDocument(token);

            // Act
            var verified = VerifySignature(token);
            
            // Assert
            Assert.That(verified);
        }

        /// <summary>
        /// Tests that the manipulation of an assertion is detected by the signature.
        /// </summary>
        [Test]
        public void ManipulatingAssertionMakesSignatureInvalid()
        {
            // Arrange
            var token = AssertionUtil.GetTestAssertion();
            SignDocument(token);

            // Manipulate the #%!;er: Attempt to remove the <AudienceRestriction> from the list of conditions.
            var conditions = (XmlElement)token.GetElementsByTagName("Conditions", "urn:oasis:names:tc:SAML:2.0:assertion")[0];
            var audienceRestriction = (XmlElement)conditions.GetElementsByTagName("AudienceRestriction", "urn:oasis:names:tc:SAML:2.0:assertion")[0];

            conditions.RemoveChild(audienceRestriction);
            
            // Act
            var verified = VerifySignature(token);

            // Assert
            Assert.IsFalse(verified);
        }

        /// <summary>
        /// Tests the signing code of the Assertion class, by first creating an unsigned assertion and then signing and 
        /// verifying it.
        /// </summary>
        /// <remarks>
        /// TODO: test data needs fixing
        /// </remarks>
        [Ignore]
        public void TestSigning03()
        {
            // Load an unsigned assertion. 
            var assertion = new Saml20Assertion(AssertionUtil.GetTestAssertion().DocumentElement, null, false);
            
            // Check that the assertion is not considered valid in any way.
            try
            {
                assertion.CheckValid(AssertionUtil.GetTrustedSigners(assertion.Issuer));
                Assert.Fail("Unsigned assertion was passed off as valid.");
            }
            catch
            {
                // Added to make resharper happy
                Assert.That(true);
            }

            var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
            Assert.That(cert.HasPrivateKey, "Certificate no longer contains a private key. Modify test.");
            assertion.Sign(cert);

            // Check that the signature is now valid
            assertion.CheckValid(new[] { cert.PublicKey.Key });
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Signs the document given as an argument.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private static void SignDocument(XmlDocument doc)
        {
            var signedXml = new SignedXml(doc);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            // TODO Dynamically dig out the correct ID attribute from the XmlDocument.
            var reference = new Reference("#_b8977dc86cda41493fba68b32ae9291d"); 

            var envelope = new XmlDsigEnvelopedSignatureTransform();                        
            reference.AddTransform(envelope);

            // NOTE: C14n may require the following list of namespace prefixes. Seems to work without it, though.
            // List<string> prefixes = new List<string>();
            // prefixes.Add(doc.DocumentElement.GetPrefixOfNamespace("http://www.w3.org/2000/09/xmldsig#"));
            // prefixes.Add(doc.DocumentElement.GetPrefixOfNamespace("http://www.w3.org/2001/XMLSchema-instance"));
            // prefixes.Add(doc.DocumentElement.GetPrefixOfNamespace("http://www.w3.org/2001/XMLSchema"));
            // prefixes.Add(doc.DocumentElement.GetPrefixOfNamespace("urn:oasis:names:tc:SAML:2.0:assertion"));

            // XmlDsigExcC14NTransform C14NTransformer = new XmlDsigExcC14NTransform(string.Join(" ", prefixes.ToArray()).Trim());
            var c14NTransformer = new XmlDsigExcC14NTransform();

            reference.AddTransform(c14NTransformer);            
            signedXml.AddReference(reference);

            // Add the key to the signature, so the assertion can be verified by itself.
            signedXml.KeyInfo = new KeyInfo();

            // Use RSA key for signing.
            //    CspParameters parameters = new CspParameters();
            //    parameters.KeyContainerName = "XML_DSIG_RSA_KEY";
            //    RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(parameters);
            //    signedXml.SigningKey = rsaKey;
            //    signedXml.KeyInfo.AddClause(new RSAKeyValue(rsaKey));

            // Use X509 Certificate for signing.
            var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
            Assert.That(cert.HasPrivateKey);
            signedXml.SigningKey = cert.PrivateKey;
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.EndCertOnly));

            // Information on the these and other "key info clause" types can be found at:
            // ms-help://MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/CPref18/html/T_System_Security_Cryptography_Xml_KeyInfoClause_DerivedTypes.htm

            // Do it!
            signedXml.ComputeSignature();

            var nodes = doc.DocumentElement.GetElementsByTagName("Issuer", Saml20Constants.Assertion);
            Assert.That(nodes.Count == 1);
            var node = nodes[0];
            doc.DocumentElement.InsertAfter(doc.ImportNode(signedXml.GetXml(), true), node); 
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="assertionFile">The assertion file.</param>
        /// <returns>The XML document.</returns>
        private static XmlDocument LoadDocument(string assertionFile)
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
        /// Loads an assertion and tries to verify it using the key embedded in the assertion.
        /// </summary>
        /// <param name="assertionFile">Path to the file containing the assertion to verify.</param>
        /// <returns>True if the signature is valid, else false.</returns>
        private static bool VerifySignature(string assertionFile)
        {
            return VerifySignature(LoadDocument(assertionFile));
        }

        /// <summary>
        /// Verifies the signature of the assertion contained in the document given as parameter.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>True if the signature is valid, else false.</returns>
        private static bool VerifySignature(XmlDocument assertion)
        {
            var signedXml = new SignedXml(assertion.DocumentElement);

            var nodeList = assertion.GetElementsByTagName(Schema.XmlDSig.Signature.ElementName, Saml20Constants.Xmldsig);
            signedXml.LoadXml((XmlElement)nodeList[0]);

            Assert.IsNotNull(signedXml.Signature);

            return signedXml.CheckSignature();
        }

        #endregion
    }
}
