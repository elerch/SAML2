using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using CONSTS = SAML2.Bindings.HttpRedirectBindingConstants;

namespace SAML2.Bindings
{
    /// <summary>
    /// Handles the creation of redirect locations when using the HTTP redirect binding, which is outlined in [SAMLBind] 
    /// section 3.4. 
    /// </summary>
    public class HttpRedirectBindingBuilder
    {
        /// <summary>
        /// Request backing field.
        /// </summary>
        private string _request;

        /// <summary>
        /// Response backing field.
        /// </summary>
        private string _response;

        /// <summary>
        /// SigningKey backing field.
        /// </summary>
        private AsymmetricAlgorithm _signingKey;

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public string Request
        {
            get { return _request; }
            set
            {
                if (!string.IsNullOrEmpty(_response))
                {
                    throw new ArgumentException("Response property is already specified. Unable to set Request property.");
                }

                _request = value;
            }
        }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public string Response
        {
            get { return _response; }
            set
            {
                if (!string.IsNullOrEmpty(_request))
                {
                    throw new ArgumentException("Request property is already specified. Unable to set Response property.");
                }

                _response = value;
            }
        }

        /// <summary>
        /// <para>Gets or sets the relay state of the message.</para>
        /// <para>If the message being built is a response message, the relay state will be included unmodified.</para>
        /// <para>If the message being built is a request message, the relay state will be encoded and compressed before being included.</para>
        /// </summary>
        public string RelayState { get; set; }

        /// <summary>
        /// Gets or sets the signing key.
        /// </summary>
        /// <value>The signing key.</value>
        public AsymmetricAlgorithm SigningKey
        {
            get { return _signingKey; }
            set
            {
                // Check if the key is of a supported type. [SAMLBind] sect. 3.4.4.1 specifies this.
                if (!(value is RSACryptoServiceProvider || value is DSA || value == null))
                {
                    throw new ArgumentException("Signing key must be an instance of either RSACryptoServiceProvider or DSA.");
                }

                _signingKey = value;
            }
        }

        /// <summary>
        /// Returns the query part of the url that should be redirected to.
        /// The resulting string should be pre-pended with either ? or &amp; before use.
        /// </summary>
        /// <returns>The query string part of the redirect URL.</returns>
        public string ToQuery()
        {
            var result = new StringBuilder();

            AddMessageParameter(result);
            AddRelayState(result);
            AddSignature(result);

            return result.ToString();
        }

        /// <summary>
        /// Uses DEFLATE compression to compress the input value. Returns the result as a Base64 encoded string.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>The compressed string.</returns>
        private static string DeflateEncode(string val)
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false)))
            {
                writer.Write(val);
                writer.Close();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
            }
        }

        /// <summary>
        /// Uppercase the URL-encoded parts of the string. Needed because Ping does not seem to be able to handle lower-cased URL-encodings.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value with URL encodings uppercased.</returns>
        private static string UpperCaseUrlEncode(string value)
        {
            var result = new StringBuilder(value);
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i] == '%')
                {
                    result[++i] = char.ToUpper(result[i]);
                    result[++i] = char.ToUpper(result[i]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// If the RelayState property has been set, this method adds it to the query string.
        /// </summary>
        /// <param name="result">The result.</param>
        private void AddRelayState(StringBuilder result)
        {
            if (RelayState == null)
            {
                return;
            }

            result.Append("&RelayState=");

            // Encode the relay state if we're building a request. Otherwise, append unmodified.
            result.Append(_request != null ? HttpUtility.UrlEncode(DeflateEncode(RelayState)) : RelayState);
        }

        /// <summary>
        /// If an asymmetric key has been specified, sign the request.
        /// </summary>
        /// <param name="result">The result.</param>
        private void AddSignature(StringBuilder result)
        {
            if (_signingKey == null)
            {
                return;
            }
            
            result.Append(string.Format("&{0}=", HttpRedirectBindingConstants.SigAlg));

            if (_signingKey is RSA)
            {
                result.Append(UpperCaseUrlEncode(HttpUtility.UrlEncode(SignedXml.XmlDsigRSASHA1Url)));
            }
            else
            {
                result.Append(UpperCaseUrlEncode(HttpUtility.UrlEncode(SignedXml.XmlDsigDSAUrl)));
            }

            // Calculate the signature of the URL as described in [SAMLBind] section 3.4.4.1.
            var signature = SignData(Encoding.UTF8.GetBytes(result.ToString()));            
            
            result.AppendFormat("&{0}=", HttpRedirectBindingConstants.Signature);
            result.Append(HttpUtility.UrlEncode(Convert.ToBase64String(signature)));
        }

        /// <summary>
        /// Create the signature for the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>SignData based on passed data and SigningKey.</returns>
        private byte[] SignData(byte[] data)
        {
            if (_signingKey is RSACryptoServiceProvider)
            {
                var rsa = (RSACryptoServiceProvider)_signingKey;
                return rsa.SignData(data, new SHA1CryptoServiceProvider());
            } 
            else
            {
                var dsa = (DSACryptoServiceProvider)_signingKey;
                return dsa.SignData(data);
            }
        }

        /// <summary>
        /// Depending on which one is specified, this method adds the SAMLRequest or SAMLResponse parameter to the URL query.
        /// </summary>
        /// <param name="result">The result.</param>
        private void AddMessageParameter(StringBuilder result)
        {
            if (!(_response == null || _request == null))
            {
                throw new Exception("Request or Response property MUST be set.");
            }

            string value; 
            if (_request != null)
            {
                result.AppendFormat("{0}=", CONSTS.SamlRequest);
                value = _request;
            }
            else
            {
                result.AppendFormat("{0}=", HttpRedirectBindingConstants.SamlResponse);
                value = _response;
            }

            var encoded = DeflateEncode(value);
            result.Append(UpperCaseUrlEncode(HttpUtility.UrlEncode(encoded)));
        }
    }
}
