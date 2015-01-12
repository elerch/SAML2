using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using SAML2.Schema.Metadata;
using SAML2.Utils;
using System.Collections.Specialized;

namespace SAML2.Bindings
{
    /// <summary>
    /// Parses the response messages related to the HTTP POST binding.
    /// </summary>
    public class HttpPostBindingParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostBindingParser"/> class.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public HttpPostBindingParser(NameValueCollection requestParams)
        {
            var base64 = string.Empty;

            if (requestParams["SAMLRequest"] != null)
            {
                base64 = requestParams["SAMLRequest"];
                IsRequest = true;
            }

            if (requestParams["SAMLResponse"] != null)
            {
                base64 = requestParams["SAMLResponse"];
                IsResponse = true;
            }

            Message = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

            Document = new XmlDocument { PreserveWhitespace = true };
            Document.LoadXml(Message);
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public XmlDocument Document { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is request.
        /// </summary>
        public bool IsRequest { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is response.
        /// </summary>
        public bool IsResponse { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the message is signed.
        /// </summary>
        public bool IsSigned
        {
            get { return XmlSignatureUtils.IsSigned(Document); }
        }
        
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Checks the signature.
        /// </summary>
        /// <returns>True of the signature is valid, else false.</returns>
        public bool CheckSignature()
        {
            return XmlSignatureUtils.CheckSignature(Document);
        }

        /// <summary>
        /// Checks the signature of the message, using a specific set of keys
        /// </summary>
        /// <param name="keys">The set of keys to check the signature against</param>
        /// <returns>True of the signature is valid, else false.</returns>
        public bool CheckSignature(IEnumerable<KeyDescriptor> keys)
        {
            foreach (var keyDescriptor in keys)
            {
                foreach (KeyInfoClause clause in (KeyInfo)keyDescriptor.KeyInfo)
                {
                    var key = XmlSignatureUtils.ExtractKey(clause);
                    if (key != null && XmlSignatureUtils.CheckSignature(Document, key))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
