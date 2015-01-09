using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Xml;
using SAML2.Config;
using SAML2.Logging;

namespace SAML2.Bindings
{
    /// <summary>
    /// Implements the HTTP SOAP binding
    /// </summary>
    public class HttpSoapBindingBuilder
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        protected static readonly IInternalLogger Logger = LoggerProvider.LoggerFor(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpSoapBindingBuilder"/> class.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public HttpSoapBindingBuilder(HttpContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets or sets the current http context
        /// </summary>
        protected HttpContext Context { get; set; }

        /// <summary>
        /// Validates the server certificate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns>True if validation of the server certificate generates no policy errors</returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        /// <summary>
        /// Sends a response message.
        /// </summary>
        /// <param name="samlMessage">The SAML message.</param>
        public void SendResponseMessage(string samlMessage)
        {
            Context.Response.ContentType = "text/xml";
            var writer = new StreamWriter(Context.Response.OutputStream);
            writer.Write(WrapInSoapEnvelope(samlMessage));
            writer.Flush();
            writer.Close();
            Context.Response.End();
        }

        /// <summary>
        /// Gets a response from the IdP based on a message.
        /// </summary>
        /// <param name="endpoint">The IdP endpoint.</param>
        /// <param name="message">The message.</param>
        /// <param name="auth">Basic authentication settings.</param>
        /// <returns>The Stream.</returns>
        public Stream GetResponse(string endpoint, string message, HttpAuthElement auth)
        {
            if (auth != null && auth.ClientCertificate != null && auth.Credentials != null)
            {
                throw new Saml20Exception(string.Format("Artifact resolution cannot specify both client certificate and basic credentials for endpoint {0}", endpoint));
            }

            var binding = CreateSslBinding();
            if (auth != null && auth.ClientCertificate != null)
            {
                // Client certificate auth
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            }

            var request = Message.CreateMessage(binding.MessageVersion, HttpArtifactBindingConstants.SoapAction, new SimpleBodyWriter(message));
            request.Headers.To = new Uri(endpoint);

            var property = new HttpRequestMessageProperty { Method = "POST" };
            property.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
            
            if (auth != null && auth.Credentials != null)
            {
                // Basic http auth over ssl
                var basicAuthzHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(auth.Credentials.Username + ":" + auth.Credentials.Password));
                property.Headers.Add(HttpRequestHeader.Authorization, basicAuthzHeader);
            }
            
            request.Properties.Add(HttpRequestMessageProperty.Name, property);
            if (Context.Request.Params["relayState"] != null)
            {
                request.Properties.Add("relayState", Context.Request.Params["relayState"]);
            }          
  
            var epa = new EndpointAddress(endpoint);

            var factory = new ChannelFactory<IRequestChannel>(binding, epa);
            if (auth != null && auth.ClientCertificate != null)
            {
                // Client certificate
                factory.Credentials.ClientCertificate.Certificate = auth.ClientCertificate.GetCertificate();
            }

            var reqChannel = factory.CreateChannel();
            
            reqChannel.Open();
            var response = reqChannel.Request(request);
            Console.WriteLine(response);
            reqChannel.Close();

            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.Load(response.GetReaderAtBodyContents());
            var outerXml = doc.DocumentElement.OuterXml;
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(outerXml));

            return memStream;
        }

        /// <summary>
        /// Wraps a message in a SOAP envelope.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The wrapped message.</returns>
        public string WrapInSoapEnvelope(string message)
        {
            var builder = new StringBuilder();

            builder.AppendLine(SoapConstants.EnvelopeBegin);
            builder.AppendLine(SoapConstants.BodyBegin);
            builder.AppendLine(message);
            builder.AppendLine(SoapConstants.BodyEnd);
            builder.AppendLine(SoapConstants.EnvelopeEnd);

            return builder.ToString();
        }

        /// <summary>
        /// Creates a WCF SSL binding.
        /// </summary>
        /// <returns>The WCF SSL binding.</returns>
        private static BasicHttpBinding CreateSslBinding()
        {
            return new BasicHttpBinding(BasicHttpSecurityMode.Transport) { TextEncoding = Encoding.UTF8 };
        }

        /// <summary>
        /// A simple body writer
        /// </summary>
        internal class SimpleBodyWriter : BodyWriter
        {
            /// <summary>
            /// The message.
            /// </summary>
            private readonly string _message;

            /// <summary>
            /// Initializes a new instance of the <see cref="SimpleBodyWriter"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public SimpleBodyWriter(string message)
                : base(false)
            {
                _message = message;
            }

            /// <summary>
            /// When implemented, provides an extensibility point when the body contents are written.
            /// </summary>
            /// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter"/> used to write out the message body.</param>
            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteRaw(_message);
            }
        }
    }
}