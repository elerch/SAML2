using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Bindings
{
    /// <summary>
    /// Parses messages pertaining to the HTTP SOAP binding.
    /// </summary>
    public class HttpSoapBindingParser
    {
        /// <summary>
        /// The current logout request
        /// </summary>
        private LogoutRequest _logoutRequest;
        
        /// <summary>
        /// The current SAML message
        /// </summary>
        private XmlElement _samlMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSoapBindingParser"/> class.
        /// </summary>
        /// <param name="httpInputStream">The HTTP input stream.</param>
        public HttpSoapBindingParser(Stream httpInputStream)
        {
            InputStream = httpInputStream;
        }

        /// <summary>
        /// Gets a value indicating whether the current message is a LogoutRequest.
        /// <c>true</c> if the current message is a LogoutRequest; otherwise, <c>false</c>.
        /// </summary>
        public bool IsLogoutReqest
        {
            get { return SamlMessageName == LogoutRequest.ElementName; }
        }

        /// <summary>
        /// Gets the LogoutRequest message.
        /// </summary>
        /// <value>The logout request.</value>
        public LogoutRequest LogoutRequest
        {
            get
            {
                if (!IsLogoutReqest)
                {
                    throw new InvalidOperationException("The SAML message is not an LogoutRequest");
                }

                LoadLogoutRequest();
                
                return _logoutRequest;
            }
        }

        /// <summary>
        /// Gets the current SAML message.
        /// </summary>
        /// <value>The SAML message.</value>
        public XmlElement SamlMessage
        {
            get
            {
                LoadSamlMessage();
                return _samlMessage;
            }
        }

        /// <summary>
        /// Gets the name of the SAML message.
        /// </summary>
        /// <value>The name of the SAML message.</value>
        public string SamlMessageName
        {
            get
            {
                return SamlMessage.LocalName;
            }
        }

        /// <summary>
        /// Gets or sets the input stream.
        /// </summary>
        /// <value>The input stream.</value>
        protected Stream InputStream { get; set; }

        /// <summary>
        /// Gets or sets the SOAP envelope.
        /// </summary>
        /// <value>The SOAP envelope.</value>
        protected string SoapEnvelope { get; set; }

        /// <summary>
        /// Checks the SAML message signature.
        /// </summary>
        /// <param name="keys">The keys to check the signature against.</param>
        /// <returns>True if the signature is valid, else false.</returns>
        public bool CheckSamlMessageSignature(List<KeyDescriptor> keys)
        {
            foreach (var keyDescriptor in keys)
            {
                foreach (KeyInfoClause clause in (KeyInfo)keyDescriptor.KeyInfo)
                {
                    var key = XmlSignatureUtils.ExtractKey(clause);
                    if (key != null && CheckSignature(key))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the status of the current message.
        /// </summary>
        /// <returns>The <see cref="Status"/>.</returns>
        public Status GetStatus()
        {
            var status = (XmlElement)SamlMessage.GetElementsByTagName(Status.ElementName, Saml20Constants.Protocol)[0];
            return status != null ? Serialization.Deserialize<Status>(new XmlNodeReader(status)) : null;
        }

        /// <summary>
        /// Loads the SAML message.
        /// </summary>
        protected void LoadSamlMessage()
        {
            if (_samlMessage != null)
            {
                return;
            }

            var reader = new StreamReader(InputStream);
            SoapEnvelope = reader.ReadToEnd();

            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(SoapEnvelope);

            var soapBody = (XmlElement)doc.GetElementsByTagName(SoapConstants.SoapBody, SoapConstants.SoapNamespace)[0];

            _samlMessage = soapBody != null ? (XmlElement)soapBody.FirstChild : doc.DocumentElement;
        }

        /// <summary>
        /// Checks the signature.
        /// </summary>
        /// <param name="key">The key to check against.</param>
        /// <returns>True if the signature is valid, else false.</returns>
        private bool CheckSignature(AsymmetricAlgorithm key)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(SamlMessage.OuterXml);

            return XmlSignatureUtils.CheckSignature(doc, key);
        }

        /// <summary>
        /// Loads the current message as a LogoutRequest.
        /// </summary>
        private void LoadLogoutRequest()
        {
            if (_logoutRequest == null)
            {
                _logoutRequest = Serialization.Deserialize<LogoutRequest>(new XmlNodeReader(SamlMessage));
            }
        }
    }
}
