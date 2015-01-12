using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;
using SAML2.Validation;

namespace SAML2
{
    /// <summary>
    /// Encapsulates the functionality required of a DK-SAML 2.0 Assertion. 
    /// </summary>
    public class Saml20Assertion 
    {
        /// <summary>
        /// Configuration element
        /// </summary>
        private readonly Saml2Configuration _config = null;

        /// <summary>
        /// Auto validate assertions.
        /// </summary>
        private readonly bool _autoValidate = true;

        /// <summary>
        /// The profile.
        /// </summary>
        private readonly string _profile;

        /// <summary>
        /// Quirks mode switch.
        /// </summary>
        private readonly bool _quirksMode;

        /// <summary>
        /// A strongly-typed version of the assertion. It is generated on-demand from the contents of the <code>_samlAssertion</code>
        /// field. 
        /// </summary>
        private Assertion _assertion;

        /// <summary>
        /// An list of the unencrypted attributes in the assertion. This list is lazy initialized, i.e. it will only be retrieved
        /// from the <code>_samlAssertion</code> field when it is requested through the <code>Attributes</code> property.
        /// When the <code>Sign</code> method is called, the attributes in the list are embedded into the <code>_samlAssertion</code>
        /// and this variable is nulled.
        /// </summary>
        private List<SamlAttribute> _assertionAttributes;

        /// <summary>
        /// The assertion validator.
        /// </summary>
        private ISaml20AssertionValidator _assertionValidator;

        /// <summary>
        /// List of encrypted assertion attributes.
        /// </summary>
        private List<EncryptedElement> _encryptedAssertionAttributes;


        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20Assertion"/> class.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="trustedSigners">If <code>null</code>, the signature of the given assertion is not verified.</param>
        /// <param name="quirksMode">if set to <c>true</c> quirks mode is enabled.</param>
        public Saml20Assertion(XmlElement assertion, IEnumerable<AsymmetricAlgorithm> trustedSigners, bool quirksMode, Saml2Configuration config)
        {
            _quirksMode = quirksMode;
            _profile = null;
            _config = config;
            LoadXml(assertion, trustedSigners, config);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20Assertion"/> class.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="trustedSigners">If <code>null</code>, the signature of the given assertion is not verified.</param>
        /// <param name="profile">Determines the type of validation to perform on the token</param>
        /// <param name="quirksMode">if set to <c>true</c> quirks mode is enabled.</param>
        public Saml20Assertion(XmlElement assertion, IEnumerable<AsymmetricAlgorithm> trustedSigners, string profile, bool quirksMode)
        {
            _profile = profile;
            _quirksMode = quirksMode;
            LoadXml(assertion, trustedSigners, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20Assertion"/> class.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="trustedSigners">If <code>null</code>, the signature of the given assertion is not verified.</param>
        /// <param name="profile">Determines the type of validation to perform on the token</param>
        /// <param name="quirksMode">if set to <c>true</c> quirks mode is enabled.</param>
        /// <param name="autoValidate">Turn automatic validation on or off</param>
        public Saml20Assertion(XmlElement assertion, IEnumerable<AsymmetricAlgorithm> trustedSigners, string profile, bool quirksMode, bool autoValidate)
        {
            _profile = profile;
            _quirksMode = quirksMode;
            _autoValidate = autoValidate;
            LoadXml(assertion, trustedSigners, null);
        }

        /// <summary>
        /// Value of current time, usually needed only for unit tests
        /// </summary>
        internal static DateTime? AlternateNow { private get; set; }

        private DateTime Now
        {
            get { return (AlternateNow ?? (DateTime?)DateTime.Now.ToUniversalTime()).Value; }
        }

        /// <summary>
        /// Gets a strongly-typed version of the SAML Assertion. It is lazily generated based on the contents of the
        /// <code>_samlAssertion</code> field.
        /// </summary>
        public Assertion Assertion
        {
            get
            {
                if (_assertion == null)
                {
                    if (XmlAssertion == null)
                    {
                        throw new InvalidOperationException("No assertion is loaded.");
                    }

                    _assertion = Serialization.Deserialize<Assertion>(new XmlNodeReader(XmlAssertion));
                }

                return _assertion;
            }
        }

        /// <summary>
        /// Gets or sets the unencrypted attributes of the assertion.
        /// </summary>
        public List<SamlAttribute> Attributes
        {
            get
            {
                if (_assertionAttributes == null)
                {
                    ExtractAttributes(); // Lazy initialization of the attributes list.                
                }

                return _assertionAttributes;
            }

            set
            {
                // _assertionAttributes == null is reserved for signalling that the attribute is not initialized, so 
                // convert it to an empty list.
                if (value == null)
                {
                    value = new List<SamlAttribute>(0);
                }

                _assertionAttributes = value;
            }
        }

        /// <summary>
        /// Gets the conditions element of the assertion.
        /// </summary>
        /// <value>The conditions element.</value>
        public Conditions Conditions
        {
            get
            {
                return _assertion.Conditions;
            }
        }

        /// <summary>
        /// Gets or sets the encrypted attributes of the assertion.
        /// </summary>
        /// <value>The encrypted attributes.</value>
        public List<EncryptedElement> EncryptedAttributes
        {
            get
            {
                if (_encryptedAssertionAttributes == null)
                {
                    ExtractAttributes(); // Lazy initialization of the attributes list.
                }

                return _encryptedAssertionAttributes;
            }

            set
            {
                // _encryptedAssertionAttributes == null is reserved for signalling that the attribute is not initialized, so 
                // convert it to an empty list.
                if (value == null)
                {
                    value = new List<EncryptedElement>(0);
                }

                _encryptedAssertionAttributes = value;
            }
        }

        /// <summary>
        /// Gets or sets the encrypted id.
        /// </summary>
        /// <value>The encrypted id.</value>
        public string EncryptedId { get; set; }
        
        /// <summary>
        /// Gets the ID attribute of the &lt;Assertion&gt; element.
        /// </summary>
        public string Id
        {
            get { return Assertion.Id; }
        }
        
        /// <summary>
        /// Gets a value indicating whether the expiration time has been exceeded.
        /// </summary>
        public bool IsExpired
        {
            get { return Now > NotOnOrAfter; }
        }

        /// <summary>
        /// Gets a value indicating whether the assertion has a OneTimeUse condition.
        /// </summary>
        /// <value>
        /// <c>true</c> if the assertion has a OneTimeUse condition; otherwise, <c>false</c>.
        /// </value>
        public bool IsOneTimeUse
        {
            get { return Assertion.Conditions.Items.OfType<OneTimeUse>().Any(); }
        }

        /// <summary>
        /// Gets the value of the &lt;Issuer&gt; element.
        /// </summary>
        public string Issuer
        {
            get { return Assertion.Issuer.Value; }
        }

        /// <summary>
        /// Gets the NotOnOrAfter property, if it is included in the assertion.
        /// </summary>
        public DateTime NotOnOrAfter
        {
            get
            {
                // Find the SubjectConfirmation element for the ValidTo attribute. [DKSAML] ch. 7.1.4.
                foreach (var o in Assertion.Subject.Items)
                {
                    if (o is SubjectConfirmation)
                    {
                        var subjectConfirmation = (SubjectConfirmation)o;
                        if (subjectConfirmation.SubjectConfirmationData.NotOnOrAfter.HasValue)
                        {
                            return subjectConfirmation.SubjectConfirmationData.NotOnOrAfter.Value;
                        }
                    }
                }

                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// Gets the <c>SessionIndex</c> of the <c>AuthnStatement</c>
        /// </summary>
        public string SessionIndex
        {
            get
            {
                var list = Assertion.GetAuthnStatements();
                return list.Count > 0 ? list[0].SessionIndex : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the asymmetric key that can verify the signature of the assertion.
        /// </summary>
        public AsymmetricAlgorithm SigningKey { get; set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public NameId Subject
        {
            get { return Assertion.Subject.Items.OfType<NameId>().FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the subject items.
        /// </summary>
        /// <value>The subject items.</value>
        public object[] SubjectItems
        {
            get
            {
                return Assertion.Subject.Items;
            }
        }

        /// <summary>
        /// Gets the assertion in XmlElement representation.
        /// </summary>
        /// <value>The XML assertion.</value>
        public XmlElement XmlAssertion { get; private set; }

        /// <summary>
        /// Gets the assertion validator.
        /// </summary>
        private ISaml20AssertionValidator GetAssertionValidator(Saml2Configuration config)
        {
            if (_assertionValidator == null)
            {
                if (config == null || config.AllowedAudienceUris == null)
                {
                    if (string.IsNullOrEmpty(_profile))
                    {
                        _assertionValidator = new Saml20AssertionValidator(null, _quirksMode);
                    }
                    else
                    {
                        _assertionValidator = (ISaml20AssertionValidator)Activator.CreateInstance(Type.GetType(_profile), null, _quirksMode);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_profile))
                    {
                        _assertionValidator = new Saml20AssertionValidator(config.AllowedAudienceUris.Select(x => x.Uri).ToList(), _quirksMode);
                    }
                    else
                    {
                        _assertionValidator = (ISaml20AssertionValidator)Activator.CreateInstance(Type.GetType(_profile), config.AllowedAudienceUris, _quirksMode);
                    }
                }
            }

            return _assertionValidator;
        }

        /// <summary>
        /// Check the signature of the XmlDocument using the list of keys. 
        /// If the signature key is found, the SigningKey property is set.
        /// </summary>
        /// <param name="keys">A list of KeyDescriptor elements. Probably extracted from the metadata describing the IDP that sent the message.</param>
        /// <returns>True, if one of the given keys was able to verify the signature. False in all other cases.</returns>
        public bool CheckSignature(IEnumerable<AsymmetricAlgorithm> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            return keys.Where(key => key != null).Any(CheckSignature);
        }

        /// <summary>
        /// Verifies the assertion's signature and its time to live.
        /// </summary>
        /// <param name="trustedSigners">The trusted signers.</param>
        /// <exception cref="Saml20Exception">if the assertion's signature can not be verified or its time to live has been exceeded.</exception>
        public void CheckValid(IEnumerable<AsymmetricAlgorithm> trustedSigners)
        {
            if (!CheckSignature(trustedSigners))
            {
                throw new Saml20Exception("Signature could not be verified.");
            }

            if (IsExpired)
            {
                throw new Saml20Exception("Assertion is no longer valid.");
            }
        }
       
        /// <summary>
        /// Returns the KeyInfo element of the signature of the token.
        /// </summary>
        /// <returns>Null if the token is not signed. The KeyInfo element otherwise.</returns>
        public KeyInfo GetSignatureKeys()
        {
            return !XmlSignatureUtils.IsSigned(XmlAssertion) ? null : XmlSignatureUtils.ExtractSignatureKeys(XmlAssertion);
        }

        /// <summary>
        /// Returns the SubjectConfirmationData from the assertion subject items
        /// </summary>
        /// <returns>SubjectConfirmationData object from subject items, null if none present</returns>
        public SubjectConfirmationData GetSubjectConfirmationData()
        {
            return SubjectItems.OfType<SubjectConfirmation>().Select(item => item.SubjectConfirmationData).FirstOrDefault();
        }

        /// <summary>
        /// Gets the assertion as an XmlDocument.
        /// </summary>
        /// <returns>The Xml of the assertion.</returns>
        public XmlElement GetXml()
        {
            return XmlAssertion;
        }

        /// <summary>
        /// Signs the assertion with the given certificate.
        /// </summary>
        /// <param name="cert">The certificate to sign the assertion with.</param>
        public void Sign(X509Certificate2 cert, Saml2Configuration config)
        {
            CheckCertificateCanSign(cert);            

            // Clear the strongly typed version of the assertion in preparation for a new source.
            _assertion = null;

            // Merge the modified attributes to the assertion.
            InsertAttributes();

            // Remove existing signatures when resigning the assertion
            var signatureParentNode = XmlAssertion; // FIX.DocumentElement;
            XmlNode sigNode;
            while ((sigNode = signatureParentNode.GetElementsByTagName(Schema.XmlDSig.Signature.ElementName, Saml20Constants.Xmldsig)[0]) != null)
            {
                signatureParentNode.RemoveChild(sigNode);
            }

            var assertionDocument = new XmlDocument();
            assertionDocument.Load(new StringReader(Serialization.SerializeToXmlString(XmlAssertion)));

            AddSignature(assertionDocument, cert);

            LoadXml(assertionDocument.DocumentElement, new List<AsymmetricAlgorithm>(new[] { cert.PublicKey.Key }), config);
        }

        /// <summary>
        /// Writes the token to a writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteAssertion(XmlWriter writer)
        {
            XmlAssertion.WriteTo(writer);
        }

        /// <summary>
        /// Adds the signature.
        /// </summary>
        /// <param name="assertionDocument">The assertion document.</param>
        /// <param name="cert">The cert.</param>
        private static void AddSignature(XmlDocument assertionDocument, X509Certificate2 cert)
        {
            var signedXml = new SignedXml(assertionDocument);
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            signedXml.SigningKey = cert.PrivateKey;

            // Retrieve the value of the "ID" attribute on the root assertion element.
            var list = assertionDocument.GetElementsByTagName(Assertion.ElementName, Saml20Constants.Assertion);
            var el = (XmlElement)list[0];            
            var reference = new Reference("#" + el.GetAttribute("ID"));

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());            
            reference.AddTransform(new XmlDsigExcC14NTransform());            

            signedXml.AddReference(reference);

            // Include the public key of the certificate in the assertion.
            // signedXml.KeyInfo = new KeyInfo();
            // signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert, X509IncludeOption.WholeChain));
            signedXml.ComputeSignature();

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            var nodes = assertionDocument.DocumentElement.GetElementsByTagName("Issuer", Saml20Constants.Assertion);            
            if (nodes.Count != 1)
            {
                throw new Saml20Exception("Assertion MUST contain one <Issuer> element.");
            }

            assertionDocument.DocumentElement.InsertAfter(assertionDocument.ImportNode(signedXml.GetXml(), true), nodes[0]);
        }

        /// <summary>
        /// Checks the certificate can sign.
        /// </summary>
        /// <param name="cert">The cert.</param>
        private static void CheckCertificateCanSign(X509Certificate2 cert)
        {
            if (cert == null)
            {
                throw new ArgumentNullException("cert");
            }

            if (!cert.HasPrivateKey)
            {
                throw new Saml20Exception("The private key must be part of the certificate.");
            }
        }

        /// <summary>
        /// Checks the signature.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True, if the given key was able to verify the signature. False in all other cases.</returns>
        private bool CheckSignature(AsymmetricAlgorithm key)
        {
            if (XmlSignatureUtils.CheckSignature(XmlAssertion, key))
            {
                SigningKey = key;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts the list of attributes from the &lt;AttributeStatement&gt; of the assertion, and 
        /// stores it in <code>_assertionAttributes</code>.
        /// </summary>
        private void ExtractAttributes()
        {            
            _assertionAttributes = new List<SamlAttribute>(0);
            _encryptedAssertionAttributes = new List<EncryptedElement>(0);

            var list = XmlAssertion.GetElementsByTagName(AttributeStatement.ElementName, Saml20Constants.Assertion);
            if (list.Count == 0)
            {
                return;
            }

            // NOTE It would be nice to implement a better-performing solution where only the AttributeStatement is converted.
            // NOTE Namespace issues in the xml-schema "type"-attribute prevents this, though.
            var assertion = Serialization.Deserialize<Assertion>(new XmlNodeReader(XmlAssertion));
                        
            var attributeStatements = assertion.GetAttributeStatements();
            if (attributeStatements.Count == 0 || attributeStatements[0].Items == null)
            {
                return;
            }

            var attributeStatement = attributeStatements[0];            
            foreach (var item in attributeStatement.Items)
            {
                if (item is SamlAttribute)
                {
                    _assertionAttributes.Add((SamlAttribute)item);
                }

                if (item is EncryptedElement)
                {
                    _encryptedAssertionAttributes.Add((EncryptedElement)item);
                }
            }
        }

        /// <summary>
        /// Merges the modified attributes into <code>AttributeStatement</code> of the assertion.
        /// </summary>
        private void InsertAttributes()
        {
            if (_assertionAttributes == null)
            {
                return;
            }
            
            // Generate the new AttributeStatement
            var attributeStatement = new AttributeStatement();
            var statements = new List<object>(_encryptedAssertionAttributes.Count + _assertionAttributes.Count);
            statements.AddRange(_assertionAttributes.ToArray());
            statements.AddRange(_encryptedAssertionAttributes.ToArray());
            attributeStatement.Items = statements.ToArray();

            var list = XmlAssertion.GetElementsByTagName(AttributeStatement.ElementName, Saml20Constants.Assertion);

            if (list.Count > 0)
            {
                // Remove the old AttributeStatement.
                XmlAssertion.RemoveChild(list[0]);

                // FIX _samlAssertion.DocumentElement.RemoveChild(list[0]);
            }

            // Only insert a new AttributeStatement if there are attributes.
            if (statements.Count > 0)
            {
                // Convert the new AttributeStatement to the Document Object Model and make a silent prayer that one day we will
                // be able to make this transition in a more elegant way.
                var attributeStatementDoc = Serialization.Serialize(attributeStatement);
                var attr = XmlAssertion.OwnerDocument.ImportNode(attributeStatementDoc.DocumentElement, true);

                // Insert the new statement.
                XmlAssertion.AppendChild(attr);                
            }

            _encryptedAssertionAttributes = null;
            _assertionAttributes = null;
        }

        /// <summary>
        /// Loads an assertion from XML.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="trustedSigners">The trusted signers.</param>
        private void LoadXml(XmlElement element, IEnumerable<AsymmetricAlgorithm> trustedSigners, Saml2Configuration config)
        {
            XmlAssertion = element;
            if (trustedSigners != null)
            {
                if (!CheckSignature(trustedSigners))
                {
                    throw new Saml20Exception("Assertion signature could not be verified.");
                }
            }

            // Validate the saml20Assertion.      
            if (_autoValidate)
            {
                GetAssertionValidator(config).ValidateAssertion(Assertion);
            }
        }
    }
}
