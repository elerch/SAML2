using System;
using System.Security.Cryptography.Xml;
using System.Xml;
using NUnit.Framework;
using SAML2.Schema.Metadata;
using SAML2.Utils;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace SAML2.Tests
{
    /// <summary>
    /// <see cref="Saml20MetadataDocument"/> tests.
    /// </summary>
    [TestFixture]
    public class Saml20MetadataDocumentTests
    {
        /// <summary>
        /// Constructor tests.
        /// </summary>
        [TestFixture]
        public class ConstructorMethod
        {
            /// <summary>
            /// Verify that certificates can be extracted.
            /// </summary>
            [Test]
            public void CanExtractCertificates()
            {
                // Arrange
                var doc = new XmlDocument { PreserveWhitespace = true };
                doc.Load(@"Protocol\MetadataDocs\metadata-ADLER.xml");

                // Act
                var metadata = new Saml20MetadataDocument(doc);
                var certificateCheckResult = XmlSignatureUtils.CheckSignature(doc, (KeyInfo)metadata.Keys[0].KeyInfo);

                // Assert
                Assert.That(metadata.GetKeys(KeyTypes.Signing).Count == 1);
                Assert.That(metadata.GetKeys(KeyTypes.Encryption).Count == 1);
                Assert.That(metadata.Keys[0].Use == KeyTypes.Signing);
                Assert.That(metadata.Keys[1].Use == KeyTypes.Encryption);

                // The two certs in the metadata document happen to be identical, and are also 
                // used for signing the entire document.
                // Extract the certificate and verify the document.
                Assert.That(certificateCheckResult);
                Assert.AreEqual("ADLER_SAML20_ID", metadata.EntityId);
            }

            /// <summary>
            /// Verify that IDP endpoints can be extracted.
            /// </summary>
            [Test]
            public void CanExtractEndpoints()
            {
                // Arrange
                var doc = new XmlDocument { PreserveWhitespace = true };
                doc.Load(@"Protocol\MetadataDocs\metadata-ADLER.xml");

                // Act
                var metadata = new Saml20MetadataDocument(doc);

                // Assert
                Assert.AreEqual(2, metadata.IDPSLOEndpoints.Count);
                Assert.AreEqual(2, metadata.SSOEndpoints.Count);
            }
        }

        /// <summary>
        /// ToXml method tests.
        /// </summary>
        [TestFixture]
        public class ToXmlMethod
        {
            /// <summary>
            /// Sign an &lt;EntityDescriptor&gt; metadata element.
            /// </summary>
            /// <remakrs>
            /// This requires that the configured signing certificate for tests is in the local store!
            /// </remakrs>
            [Test]
            [Explicit]
            public void SignsXml()
            {
                // Arrange
                var doc = new Saml20MetadataDocument(true);
                var entity = doc.CreateDefaultEntity();
                entity.ValidUntil = DateTime.Now.AddDays(14);

                var certificate = new X509Certificate2(FileEmbeddedResource("SAML2.Tests.Certificates.sts_dev_certificate.pfx"), "test1234");
                // Act
                var metadata = doc.ToXml(null, certificate);
                var document = new XmlDocument { PreserveWhitespace = true };
                document.LoadXml(metadata);
                var result = XmlSignatureUtils.CheckSignature(document);

                // Assert
                Assert.That(result);
            }

            private byte[] FileEmbeddedResource(string path)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = path;

                byte[] result = null;
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (var memoryStream = new MemoryStream()) {
                    stream.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }
                return result;
            }
        }
    }
}
