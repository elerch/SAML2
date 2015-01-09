using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using NUnit.Framework;
using SAML2.Config;
using SAML2.Protocol;
using SAML2.Schema.Core;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;

namespace SAML2.Tests
{
    /// <summary>
    /// <see cref="Saml20EncryptedAssertion"/> tests.
    /// </summary>
    [TestFixture]
    public class Saml20EncryptedAssertionTests
    {
        /// <summary>
        /// Tests that it is possible to specify the algorithm of the session key.
        /// Steps: 
        /// - Create a new encrypted assertion with a specific session key algorithm that is different than the default.
        /// - Decrypt the assertion and verify that it uses the correct algorithm.
        /// - Verify that the SessionKeyAlgorithm property behaves as expected.
        /// </summary>
        [Test]
        public void CanEncryptAssertionFull()
        {
            // Arrange
            var encryptedAssertion = new Saml20EncryptedAssertion
                                         {
                                             SessionKeyAlgorithm = EncryptedXml.XmlEncAES128Url,
                                             Assertion = AssertionUtil.GetTestAssertion()
                                         };

            var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
            encryptedAssertion.TransportKey = (RSA)cert.PublicKey.Key;

            // Act
            encryptedAssertion.Encrypt();
            var encryptedAssertionXml = encryptedAssertion.GetXml();

            // Now decrypt the assertion, and verify that it recognizes the Algorithm used.
            var decrypter = new Saml20EncryptedAssertion((RSA)cert.PrivateKey);
            decrypter.LoadXml(encryptedAssertionXml.DocumentElement);

            // Set a wrong algorithm and make sure that the class gets it algorithm info from the assertion itself.
            decrypter.SessionKeyAlgorithm = EncryptedXml.XmlEncTripleDESUrl;
            decrypter.Decrypt();

            // Assert
            // Go through the children and look for the EncryptionMethod element, and verify its algorithm attribute.
            var encryptionMethodFound = false;
            foreach (XmlNode node in encryptedAssertionXml.GetElementsByTagName(Schema.XEnc.EncryptedData.ElementName, Saml20Constants.Xenc)[0].ChildNodes)
            {
                if (node.LocalName == Schema.XEnc.EncryptionMethod.ElementName && node.NamespaceURI == Saml20Constants.Xenc)
                {
                    var element = (XmlElement)node;
                    Assert.AreEqual(EncryptedXml.XmlEncAES128Url, element.GetAttribute("Algorithm"));
                    encryptionMethodFound = true;
                }
            }

            Assert.That(encryptionMethodFound, "Unable to find EncryptionMethod element in EncryptedData.");

            // Verify that the class has discovered the correct algorithm and set the SessionKeyAlgorithm property accordingly.
            Assert.AreEqual(EncryptedXml.XmlEncAES128Url, decrypter.SessionKeyAlgorithm);
            Assert.IsNotNull(decrypter.Assertion);
        }

        /// <summary>
        /// Verify there is no assertion after initialization but before decryption.
        /// </summary>
        [Test]
        public void HasNoAssertionBeforeDecrypt()
        {
            // Arrange
            var doc = AssertionUtil.LoadXmlDocument(@"Assertions\EncryptedAssertion_01");
            var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");

            // Act
            var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

            // Assert
            Assert.IsNull(encryptedAssertion.Assertion);
        }

        /// <summary>
        /// Decrypt method tests.
        /// </summary>
        [TestFixture]
        public class DecryptMethod
        {
            /// <summary>
            /// Attempts to decrypt the assertion in the file "EncryptedAssertion_01".
            /// </summary>
            [Test]
            public void CanDecryptAssertion()
            {
                // Arrange
                var doc = AssertionUtil.LoadXmlDocument(@"Assertions\EncryptedAssertion_01");
                var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

                // Act
                encryptedAssertion.Decrypt();
                var assertion = new Saml20Assertion(encryptedAssertion.Assertion.DocumentElement, null, false);

                // Assert
                Assert.IsNotNull(encryptedAssertion.Assertion);
            }

            /// <summary>
            /// Test that the <code>Saml20EncryptedAssertion</code> class is capable of finding keys that are "peer" included,
            /// i.e. the &lt;EncryptedKey&gt; element is a sibling of the &lt;EncryptedData&gt; element.
            /// </summary>
            [Test]
            public void CanDecryptAssertionWithPeerIncludedKeys()
            {
                // Arrange
                var doc = AssertionUtil.LoadXmlDocument(@"Assertions\EncryptedAssertion_02");
                var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

                // Act
                encryptedAssertion.Decrypt();

                // Assert
                Assert.IsNotNull(encryptedAssertion.Assertion);
            }

            /// <summary>
            /// Test that the <code>Saml20EncryptedAssertion</code> class is capable using 3DES keys for the session key and OAEP-padding for 
            /// the encryption of the session key.
            /// </summary>
            [Test]
            public void CanDecryptAssertionWithPeerIncluded3DesKeys()
            {
                // Arrange
                var doc = AssertionUtil.LoadXmlDocument(@"Assertions\EncryptedAssertion_04");
                var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

                // Act
                encryptedAssertion.Decrypt();

                // Assert
                Assert.IsNotNull(encryptedAssertion.Assertion);
                Assert.AreEqual(1, encryptedAssertion.Assertion.GetElementsByTagName(Assertion.ElementName, Saml20Constants.Assertion).Count);
            }

            /// <summary>
            /// Test that the <code>Saml20EncryptedAssertion</code> class is capable using AES keys for the session key and OAEP-padding for 
            /// the encryption of the session key.
            /// </summary>
            [Test]
            public void CanDecryptAssertionWithPeerIncludedAesKeys()
            {
                // Arrange
                var doc = AssertionUtil.LoadXmlDocument(@"Assertions\EncryptedAssertion_05");
                var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

                // Act
                encryptedAssertion.Decrypt();

                // Assert
                Assert.IsNotNull(encryptedAssertion.Assertion);
                Assert.AreEqual(1, encryptedAssertion.Assertion.GetElementsByTagName(Assertion.ElementName, Saml20Constants.Assertion).Count);
            }

            /// <summary>
            /// Test that the <code>Saml20EncryptedAssertion</code> class is capable of finding keys that are "peer" included,
            /// i.e. the &lt;EncryptedKey&gt; element is a sibling of the &lt;EncryptedData&gt; element, and the assertion does
            /// not specify an encryption method.
            /// </summary>
            [Test]
            public void CanDecryptAssertionWithPeerIncludedKeysWithoutSpecifiedEncryptionMethod()
            {
                // Arrange
                var doc = AssertionUtil.LoadXmlDocument(@"Assertions\EncryptedAssertion_03");
                var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

                // Act
                encryptedAssertion.Decrypt();

                // Assert
                Assert.IsNotNull(encryptedAssertion.Assertion);
            }

            /// <summary>
            /// Decrypts an assertion we received.
            /// </summary>
            /// <remarks>
            /// The entire message is Base 64 encoded in this case.
            /// </remarks>
            [Test]
            [ExpectedException(typeof(Saml20Exception), ExpectedMessage = "Assertion is no longer valid.")]
            public void CanDecryptFOBSAssertion()
            {
                // Arrange
                var doc = AssertionUtil.LoadBase64EncodedXmlDocument(@"Assertions\fobs-assertion2");
                var encryptedList = doc.GetElementsByTagName(EncryptedAssertion.ElementName, Saml20Constants.Assertion);

                // Do some mock configuration.
                var config = Saml2Config.GetConfig();
                config.AllowedAudienceUris.Add(new AudienceUriElement { Uri = "https://saml.safewhere.net" });
                config.IdentityProviders.MetadataLocation = @"Protocol\MetadataDocs\FOBS"; // Set it manually.     
                Assert.That(Directory.Exists(config.IdentityProviders.MetadataLocation));

                var cert = new X509Certificate2(@"Certificates\SafewhereTest_SFS.pfx", "test1234");
                var encryptedAssertion = new Saml20EncryptedAssertion((RSA)cert.PrivateKey);

                encryptedAssertion.LoadXml((XmlElement)encryptedList[0]);

                // Act
                encryptedAssertion.Decrypt();

                // Retrieve metadata
                var assertion = new Saml20Assertion(encryptedAssertion.Assertion.DocumentElement, null, false);
                var endp = config.IdentityProviders.FirstOrDefault(x => x.Id == assertion.Issuer);

                // Assert
                Assert.That(encryptedList.Count == 1);
                Assert.IsNotNull(endp, "Endpoint not found");
                Assert.IsNotNull(endp.Metadata, "Metadata not found");

                try
                {
                    assertion.CheckValid(AssertionUtil.GetTrustedSigners(assertion.Issuer));
                    Assert.Fail("Verification should fail. Token does not include its signing key.");
                }
                catch (InvalidOperationException)
                {
                }

                Assert.IsNull(assertion.SigningKey, "Signing key is already present on assertion. Modify test.");
                Assert.That(assertion.CheckSignature(Saml20SignonHandler.GetTrustedSigners(endp.Metadata.GetKeys(KeyTypes.Signing), endp)));
                Assert.IsNotNull(assertion.SigningKey, "Signing key was not set on assertion instance.");
            }
        }

        /// <summary>
        /// Encrypt method tests.
        /// </summary>
        [TestFixture]
        public class EncrypteMethod
        {
            /// <summary>
            /// Verify that assertions can be encrypted.
            /// </summary>
            [Test]
            public void CanEncryptAssertion()
            {
                // Arrange
                var encryptedAssertion = new Saml20EncryptedAssertion { Assertion = AssertionUtil.GetTestAssertion() };
                var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
                encryptedAssertion.TransportKey = (RSA)cert.PublicKey.Key;

                // Act
                encryptedAssertion.Encrypt();
                var encryptedAssertionXml = encryptedAssertion.GetXml();

                // Assert
                Assert.IsNotNull(encryptedAssertionXml);
                Assert.AreEqual(1, encryptedAssertionXml.GetElementsByTagName(EncryptedAssertion.ElementName, Saml20Constants.Assertion).Count);
                Assert.AreEqual(1, encryptedAssertionXml.GetElementsByTagName(Schema.XEnc.EncryptedKey.ElementName, Saml20Constants.Xenc).Count);
            }
        }

        /// <summary>
        /// SessionKeyAlgorithm property tests.
        /// </summary>
        [TestFixture]
        public class SessionKeyAlgorithmProperty
        {
            /// <summary>
            /// Verify that exception is thrown on incorrect algorithm URI being passed in.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsArgumentExceptionOnIncorrectAlgorithmUri()
            {
                // Act
                var encryptedAssertion = new Saml20EncryptedAssertion { SessionKeyAlgorithm = "RSA" };

                // Assert
                Assert.Fail("\"Saml20EncryptedAssertion\" class does not respond to incorrect algorithm identifying URI.");
            }
        }
    }
}
