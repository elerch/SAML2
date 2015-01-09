using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using NUnit.Framework;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;

namespace SAML2.Tests
{
    /// <summary>
    /// This class contains tests that can only be used when a Ping Identity server is running. 
    /// </summary>
    [TestFixture]
    [Explicit]    
    public class PingCompatibilityTest
    {
        /// <summary>
        /// Decrypts the ping assertion.
        /// </summary>
        [Test]
        public void DecryptPingAssertion()
        {
            // Load the assertion
            var doc = new XmlDocument();
            doc.Load(File.OpenRead(@"c:\tmp\pingassertion.txt"));

            var xe = GetElement(EncryptedAssertion.ElementName, Saml20Constants.Assertion, doc);

            var doc2 = new XmlDocument();
            doc2.AppendChild(doc2.ImportNode(xe, true));

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection coll = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName,
                                                                      "CN=SafewhereTest_SFS, O=Safewhere, C=DK",
                                                                      true);

            Assert.That(coll.Count == 1);

            var cert = coll[0];

            var encass = new Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc2);
            
            encass.Decrypt();

            var writer = new XmlTextWriter(Console.Out)
                             {
                                 Formatting = Formatting.Indented,
                                 Indentation = 3,
                                 IndentChar = ' '
                             };

            encass.Assertion.WriteTo(writer);
            writer.Flush();
            
            var assertion = new Saml20Assertion(encass.Assertion.DocumentElement, AssertionUtil.GetTrustedSigners(encass.Assertion.Attributes["Issuer"].Value), false);

            Assert.That(encass.Assertion != null);

            Console.WriteLine();
            foreach (SamlAttribute attribute in assertion.Attributes)
            {
                Console.WriteLine(attribute.Name + " : " + attribute.AttributeValue[0]);
            }
        }

        /// <summary>
        /// Gets the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="ns">The ns.</param>
        /// <param name="doc">The doc.</param>
        /// <returns>The specified element from the document.</returns>
        private static XmlElement GetElement(string element, string ns, XmlDocument doc)
        {
            var list = doc.GetElementsByTagName(element, ns);
            Assert.That(list.Count == 1);

            return (XmlElement)list[0];
        }
    }
}