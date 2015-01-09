using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using NUnit.Framework;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Tests
{
    /// <summary>
    /// Provides methods for generating new test data.
    /// </summary>
    public class EncryptedAssertionUtil
    {
        /// <summary>
        /// An example on how to decrypt an encrypted assertion.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void DecryptAssertion(string file)
        {
            var doc = new XmlDocument();
            doc.Load(file);            
            var encryptedDataElement = GetElement(Schema.XEnc.EncryptedData.ElementName, Saml20Constants.Xenc, doc);                        

            var encryptedData = new EncryptedData();
            encryptedData.LoadXml(encryptedDataElement);

            var nodelist = doc.GetElementsByTagName(Schema.XmlDSig.KeyInfo.ElementName, Saml20Constants.Xmldsig);
            Assert.That(nodelist.Count > 0);

            var key = new KeyInfo();
            key.LoadXml((XmlElement)nodelist[0]);

            // Review: Is it possible to figure out which certificate to load based on the Token?
            /*
             * Comment:
             * It would be possible to provide a key/certificate identifier in the EncryptedKey element, which contains the "recipient" attribute.
             * The implementation (Safewhere.Tokens.Saml20.Saml20EncryptedAssertion) currently just expects an appropriate asymmetric key to be provided,
             * and is not not concerned about its origin. 
             * If the need arises, we can easily extend the Saml20EncryptedAssertion class with a property that allows extraction key info, eg. the "recipient" 
             * attribute.
             */
            var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");

            // ms-help://MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.NETDEVFX.v20.en/CPref18/html/T_System_Security_Cryptography_Xml_KeyInfoClause_DerivedTypes.htm
            // Look through the list of KeyInfo elements to find the encrypted key.
            SymmetricAlgorithm symmetricKey = null;
            foreach (KeyInfoClause keyInfoClause in key)
            {
                if (keyInfoClause is KeyInfoEncryptedKey)
                {
                    var keyInfoEncryptedKey = (KeyInfoEncryptedKey)keyInfoClause;
                    var encryptedKey = keyInfoEncryptedKey.EncryptedKey;
                    symmetricKey = new RijndaelManaged
                                       {
                                           Key = EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, (RSA)cert.PrivateKey, false)
                                       };
                }
            }

            // Explode if we didn't manage to find a viable key.
            Assert.IsNotNull(symmetricKey);
            var encryptedXml = new EncryptedXml();
            var plaintext = encryptedXml.DecryptData(encryptedData, symmetricKey);

            var assertion = new XmlDocument();
            assertion.Load(new StringReader(System.Text.Encoding.UTF8.GetString(plaintext)));

            // A very simple test to ensure that there is indeed an assertion in the plaintext.
            Assert.AreEqual(Assertion.ElementName, assertion.DocumentElement.LocalName);
            Assert.AreEqual(Saml20Constants.Assertion, assertion.DocumentElement.NamespaceURI);

            // At this point, assertion will contain a decrypted assertion.
        }

        /// <summary>
        /// Generates an encrypted assertion and writes it to disk. 
        /// </summary>
        public static void GenerateEncryptedAssertion()
        {
            var assertion = AssertionUtil.GetTestAssertion();

            // Create an EncryptedData instance to hold the results of the encryption.o
            var encryptedData = new EncryptedData
                                    {
                                        Type = EncryptedXml.XmlEncElementUrl,
                                        EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url)
                                    };

            // Create a symmetric key. 
            var aes = new RijndaelManaged { KeySize = 256 };
            aes.GenerateKey();

            // Encrypt the assertion and add it to the encryptedData instance.
            var encryptedXml = new EncryptedXml();
            var encryptedElement = encryptedXml.EncryptData(assertion.DocumentElement, aes, false);
            encryptedData.CipherData.CipherValue = encryptedElement;

            // Add an encrypted version of the key used.
            encryptedData.KeyInfo = new KeyInfo();

            var encryptedKey = new EncryptedKey();

            // Use this certificate to encrypt the key.
            var cert = new X509Certificate2(@"Certificates\sts_dev_certificate.pfx", "test1234");
            var publicKeyRsa = cert.PublicKey.Key as RSA;

            Assert.IsNotNull(publicKeyRsa, "Public key of certificate was not an RSA key. Modify test.");
            encryptedKey.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);
            encryptedKey.CipherData = new CipherData(EncryptedXml.EncryptKey(aes.Key, publicKeyRsa, false));

            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));

            // Create the resulting Xml-document to hook into.
            var encryptedAssertion = new EncryptedAssertion
                                         {
                                             EncryptedData = new Schema.XEnc.EncryptedData(),
                                             EncryptedKey = new Schema.XEnc.EncryptedKey[1]
                                         };
            encryptedAssertion.EncryptedKey[0] = new Schema.XEnc.EncryptedKey();

            var result = Serialization.Serialize(encryptedAssertion);

            var encryptedDataElement = GetElement(Schema.XEnc.EncryptedData.ElementName, Saml20Constants.Xenc, result);
            EncryptedXml.ReplaceElement(encryptedDataElement, encryptedData, false);

            // At this point, result can be output to text
        }

        /// <summary>
        /// Gets the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="ns">The ns.</param>
        /// <param name="doc">The doc.</param>
        /// <returns>specified element from the document.</returns>
        private static XmlElement GetElement(string element, string ns, XmlDocument doc)
        {
            var list = doc.GetElementsByTagName(element, ns);
            Assert.That(list.Count == 1);

            return (XmlElement)list[0];
        }
    }
}
