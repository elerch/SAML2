namespace SAML2.Bindings
{
    /// <summary>
    /// Constants related to the HTTP SOAP binding
    /// </summary>
    public class SoapConstants
    {
        /// <summary>
        /// Soap action name
        /// </summary>
        public const string SoapAction = "SOAPAction";

        /// <summary>
        /// Soap body name
        /// </summary>
        public const string SoapBody = "Body";

        /// <summary>
        /// Soap namespace
        /// </summary>
        public const string SoapNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// Soap envelope begin constant
        /// </summary>
        public const string EnvelopeBegin = "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">";

        /// <summary>
        /// Soap envelope end constant
        /// </summary>
        public const string EnvelopeEnd = "</SOAP-ENV:Envelope>";

        /// <summary>
        /// soap body begin constant
        /// </summary>
        public const string BodyBegin = "<SOAP-ENV:Body>";

        /// <summary>
        /// Soap body end constant
        /// </summary>
        public const string BodyEnd = "</SOAP-ENV:Body>";
    }
}