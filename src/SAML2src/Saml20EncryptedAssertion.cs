using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Handles the <code>EncryptedAssertion</code> element. 
    /// </summary>
    public class Saml20EncryptedAssertion
    {
        /// <summary>
        /// Whether to use OAEP (Optimal Asymmetric Encryption Padding) by default, if no EncryptionMethod is specified 
        /// on the &lt;EncryptedKey&gt; element.
        /// </summary>
        private const bool UseOaepDefault = false;

        /// <summary>
        /// The <code>EncryptedAssertion</code> element containing an <code>Assertion</code>.
        /// </summary>
        private XmlDocument _encryptedAssertion;

        /// <summary>
        /// The session key.
        /// </summary>
        private SymmetricAlgorithm _sessionKey;

        /// <summary>
        /// Session key algorithm.
        /// </summary>
        private string _sessionKeyAlgorithm = EncryptedXml.XmlEncAES256Url;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20EncryptedAssertion"/> class.
        /// </summary>
        public Saml20EncryptedAssertion() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20EncryptedAssertion"/> class.
        /// </summary>
        /// <param name="transportKey">The transport key is used for securing the symmetric key that has encrypted the assertion.</param>        
        public Saml20EncryptedAssertion(RSA transportKey) : this()
        {
            TransportKey = transportKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20EncryptedAssertion"/> class.
        /// </summary>
        /// <param name="transportKey">The transport key is used for securing the symmetric key that has encrypted the assertion.</param>
        /// <param name="encryptedAssertion">An <code>XmlDocument</code> containing an <code>EncryptedAssertion</code> element.</param>
        public Saml20EncryptedAssertion(RSA transportKey, XmlDocument encryptedAssertion) : this(transportKey)
        {
            LoadXml(encryptedAssertion.DocumentElement);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <code>Assertion</code> element that is embedded within the <code>EncryptedAssertion</code> element.
        /// </summary>
        public XmlDocument Assertion { get; set; }

        /// <summary>
        /// Gets or sets the algorithm to use for the session key. The algorithm is specified using the identifiers given in the 
        /// Xml Encryption Specification. see also <c>http://www.w3.org/TR/xmlenc-core/#sec-Algorithms</c>
        /// The class <code>EncryptedXml</code> contains public fields with the identifiers. If nothing is 
        /// specified, a 256 bit AES key is used.
        /// </summary>
        public string SessionKeyAlgorithm
        {
            get { return _sessionKeyAlgorithm; }
            set
            {
                // Validate that the URI used to identify the algorithm of the session key is probably correct. Not a complete validation, but should catch most obvious mistakes.
                if (!value.StartsWith(Saml20Constants.Xenc))
                {
                    throw new ArgumentException("The session key algorithm must be specified using the identifying URIs listed in the specification.");
                }

                _sessionKeyAlgorithm = value;
            }
        }

        /// <summary>
        /// Gets or sets the transport key is used for securing the symmetric key that has encrypted the assertion.
        /// </summary>
        public RSA TransportKey { get; set; }

        /// <summary>
        /// Gets the key used for encrypting the <code>Assertion</code>. This key is embedded within a <code>KeyInfo</code> element
        /// in the <code>EncryptedAssertion</code> element. The session key is encrypted with the <code>TransportKey</code> before
        /// being embedded.
        /// </summary>
        private SymmetricAlgorithm SessionKey
        {
            get
            {
                if (_sessionKey == null)
                {
                    _sessionKey = GetKeyInstance(_sessionKeyAlgorithm);
                    _sessionKey.GenerateKey();
                }

                return _sessionKey;
            }
        }

        #endregion

        /// <summary>
        /// Decrypts the assertion using the key given as the method parameter. The resulting assertion
        /// is available through the <code>Assertion</code> property.
        /// </summary>
        /// <exception cref="Saml20FormatException">Thrown if it not possible to decrypt the assertion.</exception>
        public void Decrypt()
        {
            if (TransportKey == null)
            {
                throw new InvalidOperationException("The \"TransportKey\" property must contain the asymmetric key to decrypt the assertion.");
            }

            if (_encryptedAssertion == null)
            {
                throw new InvalidOperationException("Unable to find the <EncryptedAssertion> element. Use a constructor or the LoadXml - method to set it.");
            }

            var encryptedDataElement = GetElement(Schema.XEnc.EncryptedData.ElementName, Saml20Constants.Xenc, _encryptedAssertion.DocumentElement);
            var encryptedData = new EncryptedData();
            encryptedData.LoadXml(encryptedDataElement);

            SymmetricAlgorithm sessionKey;
            if (encryptedData.EncryptionMethod != null)
            {
                _sessionKeyAlgorithm = encryptedData.EncryptionMethod.KeyAlgorithm;
                sessionKey = ExtractSessionKey(_encryptedAssertion, _sessionKeyAlgorithm);
            }
            else
            {
                sessionKey = ExtractSessionKey(_encryptedAssertion);
            }

            /*
             * NOTE: 
             * The EncryptedXml class can't handle an <EncryptedData> element without an underlying <EncryptionMethod> element,
             * despite the standard dictating that this is ok. 
             * If this becomes a problem with other IDPs, consider adding a default EncryptionMethod instance manually before decrypting.
             */
            var encryptedXml = new EncryptedXml();
            var plaintext = encryptedXml.DecryptData(encryptedData, sessionKey);

            Assertion = new XmlDocument { PreserveWhitespace = true };
            try
            {
                Assertion.Load(new StringReader(Encoding.UTF8.GetString(plaintext)));
            }
            catch (XmlException e)
            {
                Assertion = null;
                throw new Saml20FormatException("Unable to parse the decrypted assertion.", e);
            }
        }

        /// <summary>
        /// Encrypts the Assertion in the assertion property and creates an <code>EncryptedAssertion</code> element
        /// that can be retrieved using the <code>GetXml</code> method.
        /// </summary>
        public void Encrypt()
        {
            if (TransportKey == null)
            {
                throw new InvalidOperationException("The \"TransportKey\" property is required to encrypt the assertion.");
            }
            
            if (Assertion == null)
            {
                throw new InvalidOperationException("The \"Assertion\" property is required for this operation.");
            }

            var encryptedData = new EncryptedData
                                    {
                                        Type = EncryptedXml.XmlEncElementUrl,
                                        EncryptionMethod = new EncryptionMethod(_sessionKeyAlgorithm)
                                    };

            // Encrypt the assertion and add it to the encryptedData instance.
            var encryptedXml = new EncryptedXml();
            var encryptedElement = encryptedXml.EncryptData(Assertion.DocumentElement, SessionKey, false);
            encryptedData.CipherData.CipherValue = encryptedElement;

            // Add an encrypted version of the key used.
            encryptedData.KeyInfo = new KeyInfo();

            var encryptedKey = new EncryptedKey
                                   {
                                       EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url),
                                       CipherData = new CipherData(EncryptedXml.EncryptKey(SessionKey.Key, TransportKey, false))
                                   };
            encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));

            // Create an empty EncryptedAssertion to hook into.
            var encryptedAssertion = new EncryptedAssertion { EncryptedData = new Schema.XEnc.EncryptedData() };

            var result = new XmlDocument();
            result.LoadXml(Serialization.SerializeToXmlString(encryptedAssertion));

            var encryptedDataElement = GetElement(Schema.XEnc.EncryptedData.ElementName, Saml20Constants.Xenc, result.DocumentElement);
            EncryptedXml.ReplaceElement(encryptedDataElement, encryptedData, false);

            _encryptedAssertion = result;
        }

        /// <summary>
        /// Returns the XML representation of the encrypted assertion.
        /// </summary>
        /// <returns>The encrypted assertion XML.</returns>
        public XmlDocument GetXml()
        {
            return _encryptedAssertion;
        }

        /// <summary>
        /// Initializes the instance with a new <code>EncryptedAssertion</code> element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void LoadXml(XmlElement element)
        {
            CheckEncryptedAssertionElement(element);

            _encryptedAssertion = new XmlDocument();
            _encryptedAssertion.AppendChild(_encryptedAssertion.ImportNode(element, true));
        }

        /// <summary>
        /// Writes the assertion to the XmlWriter.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteAssertion(XmlWriter writer)
        {
            _encryptedAssertion.WriteTo(writer);
        }

        /// <summary>
        /// Verifies that the given <code>XmlElement</code> is actually a SAML 2.0 <code>EncryptedAssertion</code> element.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void CheckEncryptedAssertionElement(XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.LocalName != EncryptedAssertion.ElementName)
            {
                throw new ArgumentException("The element must be of type \"EncryptedAssertion\".");
            }

            if (element.NamespaceURI != Saml20Constants.Assertion)
            {
                throw new ArgumentException("The element must be of type \"" + Saml20Constants.Assertion + "#EncryptedAssertion\".");
            }
        }

        /// <summary>
        /// Utility method for retrieving a single element from a document.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementNS">The element namespace.</param>
        /// <param name="doc">The doc.</param>
        /// <returns>The desired element.</returns>
        private static XmlElement GetElement(string element, string elementNS, XmlElement doc)
        {
            var list = doc.GetElementsByTagName(element, elementNS);
            return list.Count == 0 ? null : (XmlElement)list[0];
        }

        /// <summary>
        /// Creates an instance of a symmetric key, based on the algorithm identifier found in the Xml Encryption standard.
        /// see also <c>http://www.w3.org/TR/xmlenc-core/#sec-Algorithms</c>
        /// </summary>
        /// <param name="algorithm">A string containing one of the algorithm identifiers found in the XML Encryption standard. The class
        /// <code>EncryptedXml</code> contains the identifiers as fields.</param>
        /// <returns>The <see cref="SymmetricAlgorithm"/>.</returns>
        private static SymmetricAlgorithm GetKeyInstance(string algorithm)
        {
            SymmetricAlgorithm result;
            switch (algorithm)
            {
                case EncryptedXml.XmlEncTripleDESUrl:
                    result = TripleDES.Create();
                    break;
                case EncryptedXml.XmlEncAES128Url:
                    result = new RijndaelManaged { KeySize = 128 };
                    break;
                case EncryptedXml.XmlEncAES192Url:
                    result = new RijndaelManaged { KeySize = 192 };
                    break;
                case EncryptedXml.XmlEncAES256Url:
                default:
                    result = new RijndaelManaged { KeySize = 256 };
                    break;
            }

            return result;
        }

        /// <summary>
        /// An overloaded version of ExtractSessionKey that does not require a keyAlgorithm.
        /// </summary>
        /// <param name="encryptedAssertionDoc">The encrypted assertion doc.</param>
        /// <returns>The <see cref="SymmetricAlgorithm"/>.</returns>
        private SymmetricAlgorithm ExtractSessionKey(XmlDocument encryptedAssertionDoc)
        {
            return ExtractSessionKey(encryptedAssertionDoc, string.Empty);
        }

        /// <summary>
        /// Locates and deserializes the key used for encrypting the assertion. Searches the list of keys below the &lt;EncryptedAssertion&gt; element and
        /// the &lt;KeyInfo&gt; element of the &lt;EncryptedData&gt; element.
        /// </summary>
        /// <param name="encryptedAssertionDoc">The encrypted assertion doc.</param>
        /// <param name="keyAlgorithm">The XML Encryption standard identifier for the algorithm of the session key.</param>
        /// <returns>A <code>SymmetricAlgorithm</code> containing the key if it was successfully found. Null if the method was unable to locate the key.</returns>
        private SymmetricAlgorithm ExtractSessionKey(XmlDocument encryptedAssertionDoc, string keyAlgorithm)
        {
            // Check if there are any <EncryptedKey> elements immediately below the EncryptedAssertion element.
            foreach (XmlNode node in encryptedAssertionDoc.DocumentElement.ChildNodes)            
            {
                if (node.LocalName == Schema.XEnc.EncryptedKey.ElementName && node.NamespaceURI == Saml20Constants.Xenc)
                {
                    return ToSymmetricKey((XmlElement)node, keyAlgorithm);
                }
            }

            // Check if the key is embedded in the <EncryptedData> element.
            var encryptedData = GetElement(Schema.XEnc.EncryptedData.ElementName, Saml20Constants.Xenc, encryptedAssertionDoc.DocumentElement);
            if (encryptedData != null)
            {
                var encryptedKeyElement = GetElement(Schema.XEnc.EncryptedKey.ElementName, Saml20Constants.Xenc, encryptedAssertionDoc.DocumentElement);
                if (encryptedKeyElement != null)
                {
                    return ToSymmetricKey(encryptedKeyElement, keyAlgorithm);
                }
            }

            throw new Saml20FormatException("Unable to locate assertion decryption key.");
        }

        /// <summary>
        /// Extracts the key from a &lt;EncryptedKey&gt; element.
        /// </summary>
        /// <param name="encryptedKeyElement">The encrypted key element.</param>
        /// <param name="keyAlgorithm">The key algorithm.</param>
        /// <returns>The <see cref="SymmetricAlgorithm"/>.</returns>
        private SymmetricAlgorithm ToSymmetricKey(XmlElement encryptedKeyElement, string keyAlgorithm)
        {
            var encryptedKey = new EncryptedKey();
            encryptedKey.LoadXml(encryptedKeyElement);

            var useOaep = UseOaepDefault;
            if (encryptedKey.EncryptionMethod != null)
            {
                useOaep = encryptedKey.EncryptionMethod.KeyAlgorithm == EncryptedXml.XmlEncRSAOAEPUrl;
            }

            if (encryptedKey.CipherData.CipherValue != null)
            {
                var key = GetKeyInstance(keyAlgorithm);
                key.Key = EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue, TransportKey, useOaep);

                return key;
            }
            
            throw new NotImplementedException("Unable to decode CipherData of type \"CipherReference\".");
        }
    }
}
