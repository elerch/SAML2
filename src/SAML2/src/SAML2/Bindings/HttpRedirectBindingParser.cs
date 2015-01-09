using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using SAML2.Schema.Metadata;
using SAML2.Utils;

namespace SAML2.Bindings
{
    /// <summary>
    /// Parses and validates the query parameters of a HttpRedirectBinding. [SAMLBind] section 3.4.
    /// </summary>
    public class HttpRedirectBindingParser
    {
        /// <summary>
        /// <c>RelaystateDecoded</c> backing field.
        /// </summary>
        private string _relaystateDecoded;

        /// <summary>
        /// The signed part of the query is recreated in this string.
        /// </summary>
        private string _signedquery;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRedirectBindingParser"/> class.
        /// </summary>
        /// <param name="uri">The URL that the user was redirected to by the IDP. It is essential for the survival of the signature,
        /// that the URL is not modified in any way, e.g. by URL-decoding it.</param>
        public HttpRedirectBindingParser(Uri uri)
        {
            var paramDict = UriToDictionary(uri);
            foreach (var param in paramDict)
            {
                SetParam(param.Key, HttpUtility.UrlDecode(param.Value));
            }

            // If the message is signed, save the original, encoded parameters so that the signature can be verified.
            if (IsSigned)
            {
                CreateSignatureSubject(paramDict);
            }

            ReadMessageParameter();
        }

        /// <summary>
        /// Gets a value indicating whether the parsed message contains a request message.
        /// </summary>
        public bool IsRequest
        {
            get { return !IsResponse; }
        }

        /// <summary>
        /// Gets a value indicating whether the parsed message contains a response message.
        /// </summary>
        public bool IsResponse { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the parsed message contains a signature.
        /// </summary>
        public bool IsSigned
        {
            get { return Signature != null; }
        }

        /// <summary>
        /// Gets the message that was contained in the query. Use the <code>IsResponse</code> or the <code>IsRequest</code> property 
        /// to determine the kind of message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the relay state that was included with the query. The result will still be encoded according to the 
        /// rules given in section 3.4.4.1 of [SAMLBind], i.e. base64-encoded and DEFLATE-compressed. Use the property 
        /// <code>RelayStateDecoded</code> to get the decoded contents of the RelayState parameter.
        /// </summary>
        public string RelayState { get; private set; }

        /// <summary>
        /// Gets a decoded and decompressed version of the RelayState parameter.
        /// </summary>
        public string RelayStateDecoded
        {
            get { return _relaystateDecoded ?? (_relaystateDecoded = DeflateDecompress(RelayState)); }
        }

        /// <summary>
        /// Gets the signature value
        /// </summary>
        public string Signature { get; private set; }

        /// <summary>
        /// Gets the signature algorithm.
        /// </summary>
        /// <value>The signature algorithm.</value>
        public string SignatureAlgorithm { get; private set; }

        /// <summary>
        /// Validates the signature using the public part of the asymmetric key given as parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><code>true</code> if the signature is present and can be verified using the given key.
        /// <code>false</code> if the signature is present, but can't be verified using the given key.</returns>
        /// <exception cref="InvalidOperationException">If the query is not signed, and therefore cannot have its signature verified. Use
        /// the <code>IsSigned</code> property to check for this situation before calling this method.</exception>
        public bool CheckSignature(AsymmetricAlgorithm key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (!(key is DSA || key is RSACryptoServiceProvider))
            {
                throw new ArgumentException("The key must be an instance of either DSA or RSACryptoServiceProvider.");
            }

            if (!IsSigned)
            {
                throw new InvalidOperationException("Query is not signed, so there is no signature to verify.");
            }

            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(_signedquery));
            if (key is RSACryptoServiceProvider)
            {
                var rsa = (RSACryptoServiceProvider)key;
                return rsa.VerifyHash(hash, "SHA1", DecodeSignature());
            }
            else
            {
                var dsa = (DSA)key;
                return dsa.VerifySignature(hash, DecodeSignature());
            }
        }

        /// <summary>
        /// Check the signature of a HTTP-Redirect message using the list of keys. 
        /// </summary>
        /// <param name="keys">A list of KeyDescriptor elements. Probably extracted from the metadata describing the IDP that sent the message.</param>
        /// <returns>True, if one of the given keys was able to verify the signature. False in all other cases.</returns>
        public bool VerifySignature(IEnumerable<KeyDescriptor> keys)
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
        /// Take a Base64-encoded string, decompress the result using the DEFLATE algorithm and return the resulting
        /// string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The decompressed value.</returns>
        private static string DeflateDecompress(string value)
        {
            var encoded = Convert.FromBase64String(value);
            var memoryStream = new MemoryStream(encoded);

            var result = new StringBuilder();
            using (var stream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            {
                var testStream = new StreamReader(new BufferedStream(stream), Encoding.UTF8);

                // It seems we need to "peek" on the StreamReader to get it started. If we don't do this, the first call to 
                // ReadToEnd() will return string.empty.
                testStream.Peek();
                result.Append(testStream.ReadToEnd());

                stream.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts the URI to dictionary.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Dictionary of query parameters.</returns>
        private static Dictionary<string, string> UriToDictionary(Uri uri)
        {
            var parameters = uri.Query.Substring(1).Split('&');
            var result = new Dictionary<string, string>(parameters.Length);
            foreach (var parameter in parameters.Select(s => s.Split('=')))
            {
                result.Add(parameter[0], parameter[1]);
            }

            return result;
        }

        /// <summary>
        /// Re-creates the list of parameters that are signed, in order to verify the signature.
        /// </summary>
        /// <param name="queryParams">The query parameters.</param>
        private void CreateSignatureSubject(IDictionary<string, string> queryParams)
        {
            var signedQuery = new StringBuilder();
            if (IsResponse)
            {
                signedQuery.AppendFormat("{0}={1}", HttpRedirectBindingConstants.SamlResponse, queryParams[HttpRedirectBindingConstants.SamlResponse]);
            }
            else
            {
                signedQuery.AppendFormat("{0}={1}", HttpRedirectBindingConstants.SamlRequest, queryParams[HttpRedirectBindingConstants.SamlRequest]);
            }

            if (RelayState != null)
            {
                signedQuery.AppendFormat("&{0}={1}", HttpRedirectBindingConstants.RelayState, queryParams[HttpRedirectBindingConstants.RelayState]);
            }

            if (Signature != null)
            {
                signedQuery.AppendFormat("&{0}={1}", HttpRedirectBindingConstants.SigAlg, queryParams[HttpRedirectBindingConstants.SigAlg]);
            }

            _signedquery = signedQuery.ToString();
        }

        /// <summary>
        /// Decodes the Signature parameter.
        /// </summary>
        /// <returns>The decoded signature.</returns>
        private byte[] DecodeSignature()
        {
            if (!IsSigned)
            {
                throw new InvalidOperationException("Query does not contain a signature.");
            }

            return Convert.FromBase64String(Signature);
        }

        /// <summary>
        /// Decodes the message parameter.
        /// </summary>
        private void ReadMessageParameter()
        {
            Message = DeflateDecompress(Message);
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void SetParam(string key, string value)
        {
            switch (key.ToLower())
            {
                case "samlrequest":
                    IsResponse = false;
                    Message = value;
                    return;
                case "samlresponse":
                    IsResponse = true;
                    Message = value;
                    return;
                case "relaystate":
                    RelayState = value;
                    return;
                case "sigalg":
                    SignatureAlgorithm = value;
                    return;
                case "signature":
                    Signature = value;
                    return;
            }
        }
    }
}
