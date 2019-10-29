using System;
using System.Text;
using SAML2.Config;

namespace SAML2.Bindings
{
    /// <summary>
    /// Implementation of the HTTP POST binding.
    /// </summary>
    public class HttpPostBindingBuilder
    {
        /// <summary>
        /// The endpoint to send the message to.
        /// </summary>
        private readonly IdentityProviderEndpoint _destinationEndpoint;

        /// <summary>
        /// Request backing field.
        /// </summary>
        private string _request;

        /// <summary>
        /// Response backing field.
        /// </summary>
        private string _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostBindingBuilder"/> class.
        /// </summary>
        /// <param name="endpoint">The IdP endpoint that messages will be sent to.</param>
        public HttpPostBindingBuilder(IdentityProviderEndpoint endpoint) 
        {
            _destinationEndpoint = endpoint;
            Action = SamlActionType.SAMLRequest;
            RelayState = string.Empty;
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public SamlActionType Action { get; set; }

        /// <summary>
        /// Gets or sets the relay state
        /// </summary>
        /// <value>The relay state.</value>
        public string RelayState { get; set; }        

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
        /// Gets the ASP.Net page that will serve html to user agent.
        /// </summary>
        /// <returns>The Page.</returns>
        public string GetPage()
        {
            if (_request == null && _response == null)
            {
                throw new InvalidOperationException("A response or request message MUST be specified before generating the page.");
            }

            var msg = _request ?? _response;

            var rc = new StringBuilder(800);
            rc.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
<head>
    <title>SAML2.0 POST binding</title>
    </head>
    <body onload=""document.forms[0].submit()"">
        <noscript><prop><strong>Note:</strong> Since your browser does not support JavaScript, you must press the Continue button once to proceed.</prop></noscript>
");

            rc.AppendFormat("        <form action='{0}' method='post'><div>", _destinationEndpoint.Url);

            if (!string.IsNullOrEmpty(RelayState))
                rc.AppendFormat("            <input type='hidden' id='RelayState' name='RelayState' value='{0}'/>", RelayState);

            rc.AppendFormat("            <input type='hidden' id='{0}' name='{0}' value='{1}'/>", Enum.GetName(typeof(SamlActionType), Action), Convert.ToBase64String(Encoding.UTF8.GetBytes(msg)));

            rc.Append(@"        <noscript><div><input type=""submit"" value=""Continue""/></div></noscript>
        </div>
    </form>
</body>
</html>
");
            return rc.ToString();
        }

    }
}
