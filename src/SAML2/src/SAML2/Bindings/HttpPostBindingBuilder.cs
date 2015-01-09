using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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
        private readonly IdentityProviderEndpointElement _destinationEndpoint;

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
        public HttpPostBindingBuilder(IdentityProviderEndpointElement endpoint) 
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
        
        #region Page related functions

        /// <summary>
        /// Gets the ASP.Net page that will serve html to user agent.
        /// </summary>
        /// <returns>The Page.</returns>
        public Page GetPage()
        {
            if (_request == null && _response == null)
            {
                throw new InvalidOperationException("A response or request message MUST be specified before generating the page.");
            }

            var msg = _request ?? _response;

            var p = new Page
                        {
                            EnableViewState = false,
                            EnableViewStateMac = false
                        };

            p.Controls.Add(new LiteralControl("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine));
            p.Controls.Add(new LiteralControl("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\r\n\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">" + Environment.NewLine));

            var head = new HtmlHead { Title = "SAML2.0 POST binding" };
            p.Controls.Add(head);            

            p.Controls.Add(new LiteralControl(Environment.NewLine + "<body onload=\"document.forms[0].submit()\">" + Environment.NewLine));
            p.Controls.Add(new LiteralControl("<noscript><p><strong>Note:</strong> Since your browser does not support JavaScript, you must press the Continue button once to proceed.</p></noscript>"));                                   

            p.Controls.Add(new LiteralControl("<form action=\"" + _destinationEndpoint.Url + "\" method=\"post\"><div>"));

            if (!string.IsNullOrEmpty(RelayState))
            {
                var relayStateHidden = new HtmlInputHidden
                                           {
                                               ID = "RelayState",
                                               Name = "RelayState",
                                               Value = RelayState
                                           };
                p.Controls.Add(relayStateHidden);
            }

            var action = new HtmlInputHidden
                             {
                                 Name = Enum.GetName(typeof(SamlActionType), Action),
                                 ID = Enum.GetName(typeof(SamlActionType), Action),
                                 Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg))
                             };
            p.Controls.Add(action);

            p.Controls.Add(new LiteralControl("<noscript><div><input type=\"submit\" value=\"Continue\"/></div></noscript>"));
            p.Controls.Add(new LiteralControl("</div></form>"));                                      
            p.Controls.Add(new LiteralControl(Environment.NewLine + "</body>" + Environment.NewLine + "</html>"));

            return p;
        }

        #endregion
    }
}
