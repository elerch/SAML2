using SAML2.Bindings;
using SAML2.Logging;
using SAML2.Schema.Protocol;
using SAML2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAML2.Config;

namespace SAML2.Protocol
{
    public class Logout
    {
        private readonly Saml2Configuration config;
        private readonly IInternalLogger logger;

        public Logout(IInternalLogger logger, SAML2.Config.Saml2Configuration config)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (config == null) throw new ArgumentNullException("config"); 
            this.logger = logger;
            this.config = config;
        }
        public void ValidateLogoutRequest(string requestType, System.Collections.Specialized.NameValueCollection requestParams, Uri requestUrl)
        {
            logger.DebugFormat(TraceMessages.LogoutResponseReceived);

            var message = string.Empty;
            LogoutResponse response = null;
            switch (requestType) {
            case "GET":
                ValidateLogoutViaGet(requestUrl, out message, out response);
                break;
            case "POST":
                ValidateLogoutViaPost(requestParams, out message, out response);
                break;
            default:
                break;
            }

            if (response == null) {
                logger.ErrorFormat(ErrorMessages.UnsupportedRequestType, requestType);
                throw new Saml20Exception(string.Format(ErrorMessages.UnsupportedRequestType, requestType));
            }

            logger.DebugFormat(TraceMessages.LogoutResponseParsed, message);

            if (response.Status.StatusCode.Value != Saml20Constants.StatusCodes.Success) {
                logger.ErrorFormat(ErrorMessages.ResponseStatusNotSuccessful, response.Status.StatusCode.Value);
                throw new Saml20Exception(string.Format(ErrorMessages.ResponseStatusNotSuccessful, response.Status.StatusCode.Value));
            }
        }

        private void ValidateLogoutViaPost(System.Collections.Specialized.NameValueCollection requestParams, out string message, out LogoutResponse response)
        {
            var parser = new HttpPostBindingParser(requestParams);
            logger.DebugFormat(TraceMessages.LogoutResponsePostBindingParse, parser.Message);

            response = Serialization.DeserializeFromXmlString<LogoutResponse>(parser.Message);

            var idp = IdpSelectionUtil.RetrieveIDPConfiguration(response.Issuer.Value, config);
            if (idp.Metadata == null) {
                logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, idp.Id);
                throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, idp.Id));
            }

            if (!parser.IsSigned) {
                logger.Error(ErrorMessages.ResponseSignatureMissing);
                throw new Saml20Exception(ErrorMessages.ResponseSignatureMissing);
            }

            // signature on final message in logout
            if (!parser.CheckSignature(idp.Metadata.Keys)) {
                logger.Error(ErrorMessages.ResponseSignatureInvalid);
                throw new Saml20Exception(ErrorMessages.ResponseSignatureInvalid);
            }

            message = parser.Message;
        }

        private void ValidateLogoutViaGet(Uri requestUrl, out string message, out LogoutResponse response)
        {
            var parser = new HttpRedirectBindingParser(requestUrl);
            response = Serialization.DeserializeFromXmlString<LogoutResponse>(parser.Message);

            logger.DebugFormat(TraceMessages.LogoutResponseRedirectBindingParse, parser.Message, parser.SignatureAlgorithm, parser.Signature);

            var idp = IdpSelectionUtil.RetrieveIDPConfiguration(response.Issuer.Value, config);
            if (idp.Metadata == null) {
                logger.ErrorFormat(ErrorMessages.UnknownIdentityProvider, idp.Id);
                throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, idp.Id));
            }

            if (!parser.VerifySignature(idp.Metadata.Keys)) {
                logger.Error(ErrorMessages.ResponseSignatureInvalid);
                throw new Saml20Exception(ErrorMessages.ResponseSignatureInvalid);
            }

            message = parser.Message;
        }
    }
}
