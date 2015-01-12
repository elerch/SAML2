using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using SAML2.Config;
using System.Text;
using SAML2.Logging;
using SAML2;
using SAML2.Utils;

namespace Owin
{
    internal class SamlMetadataWriter
    {
        private Saml2Configuration configuration;
        private readonly IInternalLogger logger;

        public SamlMetadataWriter(Saml2Configuration configuration)
        {
            this.configuration = configuration;
            logger = LoggerProvider.LoggerFor(typeof(Saml2Configuration));
        }

        internal Task WriteMetadataDocument(IOwinContext context)
        {
            var encoding = Encoding.UTF8;
            string encodingStr = null;
            try {
                encodingStr = context.Request.Query.Get("encoding");
                if (!string.IsNullOrEmpty(encodingStr)) {
                    encoding = Encoding.GetEncoding(encodingStr);
                    context.Response.Headers["Content-Encoding"] = encoding.ToString();
                }
            }
            catch (ArgumentException ex) {
                logger.ErrorFormat(ErrorMessages.UnknownEncoding, encodingStr);
                throw new ArgumentException(string.Format(ErrorMessages.UnknownEncoding, encodingStr), ex);
            }

            var sign = true;
            var param = context.Request.Query.Get("sign");
            if (!string.IsNullOrEmpty(param)) {
                if (!bool.TryParse(param, out sign)) {
                    throw new ArgumentException(ErrorMessages.MetadataSignQueryParameterInvalid);
                }
            }

            context.Response.ContentType = "text/xml"; //Saml20Constants.MetadataMimetype; - that will prompt a download
            //context.Response.Headers["Content-Disposition"] = "attachment; filename=\"metadata.xml\"";

            var metautil = new MetadataUtils(configuration, logger);
            return context.Response.WriteAsync(metautil.CreateMetadataDocument(encoding, sign));
        }
    }
}