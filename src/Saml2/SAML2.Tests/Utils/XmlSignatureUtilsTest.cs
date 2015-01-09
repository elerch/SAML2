using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using SAML2.Utils;

namespace SAML2.Tests.Utils
{
    /// <summary>
    /// <see cref="XmlSignatureUtils"/> tests.
    /// </summary>
    [TestFixture]
    public class XmlSignatureUtilsTest
    {
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
        /// CheckSignature method tests.
        /// </summary>
        [TestFixture]
        public class CheckSignatureMethod
        {
            /// <summary>
            /// Verify valid signatures can be checked.
            /// </summary>
            [Test]
            public void CanCheckValidSignatures()
            {
                // Arrange
                var doc = LoadDocument(@"Assertions\Saml2Assertion_01");

                // Act
                var result = XmlSignatureUtils.CheckSignature(doc);

                // Assert
                Assert.That(result);
            }
        }

        /// <summary>
        /// ExtractSignatureKeys method tests.
        /// </summary>
        [TestFixture]
        public class ExtractSignatureKeysMethod
        {
            /// <summary>
            /// Verify signature keys can be extracted.
            /// </summary>
            [Test]
            public void CanExtractKeyInfo()
            {
                // Arrange
                var doc = LoadDocument(@"Assertions\Saml2Assertion_01");

                // Act
                var keyInfo = XmlSignatureUtils.ExtractSignatureKeys(doc);

                // Assert
                Assert.IsNotNull(keyInfo);
            }
        }

        /// <summary>
        /// IsSigned method tests.
        /// </summary>
        [TestFixture]
        public class IsSignedMethod
        {
            /// <summary>
            /// Verify signed and unsigned documents can be detected.
            /// </summary>
            [Test]
            public void CanDetectIfDocumentIsSigned()
            {
                // Arrange
                var badDocument = LoadDocument(@"Assertions\EncryptedAssertion_01");
                var goodDocument = LoadDocument(@"Assertions\Saml2Assertion_01");

                // Act
                var badResult = XmlSignatureUtils.IsSigned(badDocument);
                var goodResult = XmlSignatureUtils.IsSigned(goodDocument);

                // Assert
                Assert.IsFalse(badResult);
                Assert.IsTrue(goodResult);
            }

            /// <summary>
            /// Verify documents without preserve whitespace set will fail.
            /// </summary>
            [Test]
            [ExpectedException(typeof(InvalidOperationException))]
            public void FailsOnDocumentWithoutPreserveWhitespace()
            {
                // Arrange
                var doc = LoadDocument(@"Assertions\EncryptedAssertion_01");
                doc.PreserveWhitespace = false;

                // Act
                XmlSignatureUtils.IsSigned(doc);

                // Assert
                Assert.Fail("Signed documents that do not have PreserveWhitespace set should fail to be processed.");
            }
        }
    }
}
