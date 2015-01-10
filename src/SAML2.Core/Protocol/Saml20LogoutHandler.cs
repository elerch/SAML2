using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml;
using SAML2.Bindings;
using SAML2.Config;
using SAML2.Schema.Metadata;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Protocol
{
    /// <summary>
    /// Handles logout for all SAML bindings.
    /// </summary>
    public class Saml20LogoutHandler : Saml20AbstractEndpointHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20LogoutHandler"/> class.
        /// </summary>
        public Saml20LogoutHandler()
        {
            // Read the proper redirect url from config
            try
            {
                RedirectUrl = Saml2Config.GetConfig().ServiceProvider.Endpoints.DefaultLogoutEndpoint.RedirectUrl;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        #region IHttpHandler related
        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Handle(HttpContext context)
        {
            Handle(context, null);
        }
        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Handle(HttpContext context, Saml2Section config)
        {
            Logger.Debug(TraceMessages.LogoutHandlerCalled);

            // Some IDP's are known to fail to set an actual value in the SOAPAction header
            // so we just check for the existence of the header field.
            if (Array.Exists(context.Request.Headers.AllKeys, s => s == SoapConstants.SoapAction))
            {
                HandleSoap(context, context.Request.InputStream);
                return;
            }

            if (!string.IsNullOrEmpty(context.Request.Params["SAMLart"]))
            {
                HandleArtifact(context);
                return;
            }

            if (!string.IsNullOrEmpty(context.Request.Params["SAMLResponse"]))
            {
                HandleResponse(context, config);
            }
            else if (!string.IsNullOrEmpty(context.Request.Params["SAMLRequest"]))
            {
                HandleRequest(context);
            }
            else
            {
                IdentityProvider idpEndpoint = null;

                // context.Session[IDPLoginSessionKey] may be null if IIS has been restarted
                if (context.Session[IdpSessionIdKey] != null)
                {
                    idpEndpoint = RetrieveIDPConfiguration((string)context.Session[IdpLoginSessionKey]);
                }

                if (idpEndpoint == null)
                {
                    // TODO: Reconsider how to accomplish this.
                    context.User = null;
                    FormsAuthentication.SignOut();

                    Logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, string.Empty);
                    throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, string.Empty));
                }

                TransferClient(idpEndpoint, context);
            }
        }

        #endregion

        #region Private methods - Handlers

        /// <summary>
        /// Handles executing the logout.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="idpInitiated">if set to <c>true</c> identity provider is initiated.</param>
        private void DoLogout(HttpContext context, bool idpInitiated = false, Saml2Section config = null)
        {
            Logger.Debug(TraceMessages.LogoutActionsExecuting);
            foreach (var action in Actions.Actions.GetActions(config))
            {
                Logger.DebugFormat("{0}.{1} called", action.GetType(), "LogoutAction()");

                action.LogoutAction(this, context, idpInitiated);

                Logger.DebugFormat("{0}.{1} finished", action.GetType(), "LogoutAction()");
            }
        }

        /// <summary>
        /// Handles the artifact.
        /// </summary>
        /// <param name="context">The context.</param>
        private void HandleArtifact(HttpContext context)
        {
            var builder = new HttpArtifactBindingBuilder(context);
            var inputStream = builder.ResolveArtifact();

            HandleSoap(context, inputStream);
        }

        /// <summary>
        /// Handles the SOAP message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="inputStream">The input stream.</param>
        private void HandleSoap(HttpContext context, Stream inputStream)
        {
            var parser = new HttpArtifactBindingParser(inputStream);
            Logger.DebugFormat(TraceMessages.SOAPMessageParse, parser.SamlMessage.OuterXml);

            var builder = new HttpArtifactBindingBuilder(context);
            var config = Saml2Config.GetConfig();
            var idp = RetrieveIDPConfiguration(parser.Issuer);
            
            if (parser.IsArtifactResolve)
            {
                Logger.DebugFormat(TraceMessages.ArtifactResolveReceived, parser.SamlMessage);

                if (!parser.CheckSamlMessageSignature(idp.Metadata.Keys))
                {
                    Logger.ErrorFormat(ErrorMessages.ArtifactResolveSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ArtifactResolveSignatureInvalid);
                }

                builder.RespondToArtifactResolve(parser.ArtifactResolve);
            }
            else if (parser.IsArtifactResponse)
            {
                Logger.DebugFormat(TraceMessages.ArtifactResponseReceived, parser.SamlMessage);

                if (!parser.CheckSamlMessageSignature(idp.Metadata.Keys))
                {
                    Logger.Error(ErrorMessages.ArtifactResponseSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ArtifactResponseSignatureInvalid);
                }

                var status = parser.ArtifactResponse.Status;
                if (status.StatusCode.Value != Saml20Constants.StatusCodes.Success)
                {
                    Logger.ErrorFormat(ErrorMessages.ArtifactResponseStatusCodeInvalid, status.StatusCode.Value);
                    throw new Saml20Exception(string.Format(ErrorMessages.ArtifactResponseStatusCodeInvalid, status.StatusCode.Value));
                }

                if (parser.ArtifactResponse.Any.LocalName == LogoutRequest.ElementName)
                {
                    Logger.DebugFormat(TraceMessages.LogoutRequestReceived, parser.ArtifactResponse.Any.OuterXml);

                    var req = Serialization.DeserializeFromXmlString<LogoutRequest>(parser.ArtifactResponse.Any.OuterXml);

                    // Send logoutresponse via artifact
                    var response = new Saml20LogoutResponse
                                       {
                                           Issuer = config.ServiceProvider.Id,
                                           StatusCode = Saml20Constants.StatusCodes.Success,
                                           InResponseTo = req.Id
                                       };

                    var endpoint = RetrieveIDPConfiguration((string)context.Session[IdpLoginSessionKey]);
                    var destination = DetermineEndpointConfiguration(BindingType.Redirect, endpoint.Endpoints.DefaultLogoutEndpoint, endpoint.Metadata.IDPSLOEndpoints);

                    builder.RedirectFromLogout(destination, response);
                }
                else if (parser.ArtifactResponse.Any.LocalName == LogoutResponse.ElementName)
                {
                    DoLogout(context, false, config);
                }
                else
                {
                    Logger.ErrorFormat(ErrorMessages.ArtifactResponseMissingResponse);
                    throw new Saml20Exception(ErrorMessages.ArtifactResponseMissingResponse);
                }
            }
            else if (parser.IsLogoutReqest)
            {
                Logger.DebugFormat(TraceMessages.LogoutRequestReceived, parser.SamlMessage.OuterXml);

                var req = parser.LogoutRequest;
                
                // Build the response object
                var response = new Saml20LogoutResponse
                                   {
                                       Issuer = config.ServiceProvider.Id,
                                       StatusCode = Saml20Constants.StatusCodes.Success,
                                       InResponseTo = req.Id
                                   };

                // response.Destination = destination.Url;
                var doc = response.GetXml();
                XmlSignatureUtils.SignDocument(doc, response.Id);
                if (doc.FirstChild is XmlDeclaration)
                {
                    doc.RemoveChild(doc.FirstChild);
                }
                
                builder.SendResponseMessage(doc.OuterXml);
            }
            else
            {
                Logger.ErrorFormat(ErrorMessages.SOAPMessageUnsupportedSamlMessage);
                throw new Saml20Exception(ErrorMessages.SOAPMessageUnsupportedSamlMessage);
            }
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The context.</param>
        private void HandleRequest(HttpContext context)
        {
            Logger.DebugFormat(TraceMessages.LogoutRequestReceived);

            // Fetch the endpoint configuration
            var idp = RetrieveIDPConfiguration((string)context.Session[IdpLoginSessionKey]);
            var destination = DetermineEndpointConfiguration(BindingType.Redirect, idp.Endpoints.DefaultLogoutEndpoint, idp.Metadata.IDPSLOEndpoints);

            // Fetch config object
            var config = Saml2Config.GetConfig();

            // Build the response object
            var response = new Saml20LogoutResponse
                               {
                                   Issuer = config.ServiceProvider.Id,
                                   Destination = destination.Url,
                                   StatusCode = Saml20Constants.StatusCodes.Success
                               };

            string message;
            if (context.Request.RequestType == "GET")
            {
                // HTTP Redirect binding
                var parser = new HttpRedirectBindingParser(context.Request.Url);
                Logger.DebugFormat(TraceMessages.LogoutRequestRedirectBindingParse, parser.Message, parser.SignatureAlgorithm, parser.Signature);

                var endpoint = config.IdentityProviders.FirstOrDefault(x => x.Id == idp.Id);
                if (endpoint == null || endpoint.Metadata == null)
                {
                    Logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, idp.Id);
                    throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, idp.Id));
                }

                var metadata = endpoint.Metadata;
                if (!parser.VerifySignature(metadata.GetKeys(KeyTypes.Signing)))
                {
                    Logger.Error(ErrorMessages.RequestSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.RequestSignatureInvalid);
                }

                message = parser.Message;
            }
            else if (context.Request.RequestType == "POST")
            {
                // HTTP Post binding
                var parser = new HttpPostBindingParser(context);
                Logger.DebugFormat(TraceMessages.LogoutRequestPostBindingParse, parser.Message);

                if (!parser.IsSigned)
                {
                    Logger.Error(ErrorMessages.RequestSignatureMissing);
                    throw new Saml20Exception(ErrorMessages.RequestSignatureMissing);
                }

                var endpoint = config.IdentityProviders.FirstOrDefault(x => x.Id == idp.Id);
                if (endpoint == null || endpoint.Metadata == null)
                {
                    Logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, idp.Id);
                    throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, idp.Id));
                }

                var metadata = endpoint.Metadata;

                // Check signature
                if (!parser.CheckSignature(metadata.GetKeys(KeyTypes.Signing)))
                {
                    Logger.Error(ErrorMessages.RequestSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.RequestSignatureInvalid);
                }

                message = parser.Message;
            }
            else
            {
                // Error: We don't support HEAD, PUT, CONNECT, TRACE, DELETE and OPTIONS
                Logger.ErrorFormat(ErrorMessages.UnsupportedRequestType, context.Request.RequestType);
                throw new Saml20Exception(string.Format(ErrorMessages.UnsupportedRequestType, context.Request.RequestType));
            }

            Logger.DebugFormat(TraceMessages.LogoutRequestParsed, message);

            // Log the user out locally
            DoLogout(context, true);

            var req = Serialization.DeserializeFromXmlString<LogoutRequest>(message);
            response.InResponseTo = req.Id;

            // Respond using redirect binding
            if (destination.Binding == BindingType.Redirect)
            {
                var builder = new HttpRedirectBindingBuilder
                                  {
                                      RelayState = context.Request.Params["RelayState"],
                                      Response = response.GetXml().OuterXml,
                                      SigningKey = Saml2Config.GetConfig().ServiceProvider.SigningCertificate.GetCertificate().PrivateKey
                                  };

                Logger.DebugFormat(TraceMessages.LogoutResponseSent, builder.Response);

                context.Response.Redirect(destination.Url + "?" + builder.ToQuery(), true);
                return;
            }

            // Respond using post binding
            if (destination.Binding == BindingType.Post)
            {
                var builder = new HttpPostBindingBuilder(destination)
                                  {
                                      Action = SamlActionType.SAMLResponse
                                  };

                var responseDocument = response.GetXml();

                Logger.DebugFormat(TraceMessages.LogoutResponseSent, responseDocument.OuterXml);

                XmlSignatureUtils.SignDocument(responseDocument, response.Id);
                builder.Response = responseDocument.OuterXml;
                builder.RelayState = context.Request.Params["RelayState"];
                builder.GetPage().ProcessRequest(context);
            }
        }
        
        /// <summary>
        /// Handles the response.
        /// </summary>
        /// <param name="context">The context.</param>
        private void HandleResponse(HttpContext context, Saml2Section config)
        {
            Logger.DebugFormat(TraceMessages.LogoutResponseReceived);

            var message = string.Empty;
            LogoutResponse response = null;
            if (context.Request.RequestType == "GET")
            {
                var parser = new HttpRedirectBindingParser(context.Request.Url);
                response = Serialization.DeserializeFromXmlString<LogoutResponse>(parser.Message);

                Logger.DebugFormat(TraceMessages.LogoutResponseRedirectBindingParse, parser.Message, parser.SignatureAlgorithm, parser.Signature);

                var idp = RetrieveIDPConfiguration(response.Issuer.Value);
                if (idp.Metadata == null)
                {
                    Logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, idp.Id);
                    throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, idp.Id));
                }

                if (!parser.VerifySignature(idp.Metadata.Keys))
                {
                    Logger.Error(ErrorMessages.ResponseSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ResponseSignatureInvalid);
                }

                message = parser.Message;
            }
            else if (context.Request.RequestType == "POST")
            {
                var parser = new HttpPostBindingParser(context);
                Logger.DebugFormat(TraceMessages.LogoutResponsePostBindingParse, parser.Message);

                response = Serialization.DeserializeFromXmlString<LogoutResponse>(parser.Message);

                var idp = RetrieveIDPConfiguration(response.Issuer.Value);
                if (idp.Metadata == null)
                {
                    Logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, idp.Id);
                    throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, idp.Id));
                }

                if (!parser.IsSigned)
                {
                    Logger.Error(ErrorMessages.ResponseSignatureMissing);
                    throw new Saml20Exception(ErrorMessages.ResponseSignatureMissing);
                }

                // signature on final message in logout
                if (!parser.CheckSignature(idp.Metadata.Keys))
                {
                    Logger.Error(ErrorMessages.ResponseSignatureInvalid);
                    throw new Saml20Exception(ErrorMessages.ResponseSignatureInvalid);
                }

                message = parser.Message;
            }

            if (response == null)
            {
                Logger.ErrorFormat(ErrorMessages.UnsupportedRequestType, context.Request.RequestType);
                throw new Saml20Exception(string.Format(ErrorMessages.UnsupportedRequestType, context.Request.RequestType));
            }

            Logger.DebugFormat(TraceMessages.LogoutResponseParsed, message);

            if (response.Status.StatusCode.Value != Saml20Constants.StatusCodes.Success)
            {
                Logger.ErrorFormat(ErrorMessages.ResponseStatusNotSuccessful, response.Status.StatusCode.Value);
                throw new Saml20Exception(string.Format(ErrorMessages.ResponseStatusNotSuccessful, response.Status.StatusCode.Value));
            }

            // Log the user out locally
            DoLogout(context, false, config);
        }

        /// <summary>
        /// Transfers the client.
        /// </summary>
        /// <param name="idp">The identity provider.</param>
        /// <param name="context">The context.</param>
        private void TransferClient(IdentityProvider idp, HttpContext context)
        {
            var request = Saml20LogoutRequest.GetDefault();

            // Determine which endpoint to use from the configuration file or the endpoint metadata.
            var destination = DetermineEndpointConfiguration(BindingType.Redirect, idp.Endpoints.DefaultLogoutEndpoint, idp.Metadata.IDPSLOEndpoints);
            request.Destination = destination.Url;

            var nameIdFormat = (string)context.Session[IdpNameIdFormat];
            request.SubjectToLogOut.Format = nameIdFormat;

            // Handle POST binding
            if (destination.Binding == BindingType.Post)
            {
                var builder = new HttpPostBindingBuilder(destination);
                request.Destination = destination.Url;
                request.Reason = Saml20Constants.Reasons.User;
                request.SubjectToLogOut.Value = (string)context.Session[IdpNameId];
                request.SessionIndex = (string)context.Session[IdpSessionIdKey];

                var requestDocument = request.GetXml();
                XmlSignatureUtils.SignDocument(requestDocument, request.Id);
                builder.Request = requestDocument.OuterXml;

                Logger.DebugFormat(TraceMessages.LogoutRequestSent, idp.Id, "POST", builder.Request);

                builder.GetPage().ProcessRequest(context);
                context.Response.End();
                return;
            }

            // Handle Redirect binding
            if (destination.Binding == BindingType.Redirect)
            {
                request.Destination = destination.Url;
                request.Reason = Saml20Constants.Reasons.User;
                request.SubjectToLogOut.Value = (string)context.Session[IdpNameId];
                request.SessionIndex = (string)context.Session[IdpSessionIdKey];

                var builder = new HttpRedirectBindingBuilder
                                  {
                                      Request = request.GetXml().OuterXml,
                                      SigningKey = Saml2Config.GetConfig().ServiceProvider.SigningCertificate.GetCertificate().PrivateKey
                                  };

                var redirectUrl = destination.Url + "?" + builder.ToQuery();
                Logger.DebugFormat(TraceMessages.LogoutRequestSent, idp.Id, "REDIRECT", redirectUrl);

                context.Response.Redirect(redirectUrl, true);
                return;
            }

            // Handle Artifact binding
            if (destination.Binding == BindingType.Artifact)
            {
                request.Destination = destination.Url;
                request.Reason = Saml20Constants.Reasons.User;
                request.SubjectToLogOut.Value = (string)context.Session[IdpNameId];
                request.SessionIndex = (string)context.Session[IdpSessionIdKey];

                Logger.DebugFormat(TraceMessages.LogoutRequestSent, idp.Id, "ARTIFACT", request.GetXml().OuterXml);

                var builder = new HttpArtifactBindingBuilder(context);
                builder.RedirectFromLogout(destination, request, Guid.NewGuid().ToString("N"));
            }

            Logger.Error(ErrorMessages.EndpointBindingInvalid);
            throw new Saml20Exception(ErrorMessages.EndpointBindingInvalid);
        }

        #endregion
    }
}
