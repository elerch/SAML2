using System;

namespace SAML2.Bindings
{
    /// <summary>
    /// Constants pertaining to the artifact binding over HTTP SOAP.
    /// </summary>
    public class HttpArtifactBindingConstants
    {
        /// <summary>
        /// Soap action
        /// </summary>
        public const string SoapAction = "http://www.oasis-open.org/committees/security";

        /// <summary>
        /// Default type code
        /// </summary>
        public const short ArtifactTypeCode = 0x0004;

        /// <summary>
        /// Artifact query string name
        /// </summary>
        public const string ArtifactQueryStringName = "SAMLart";

        /// <summary>
        /// Name of artifact resolve
        /// </summary>
        public const string ArtifactResolve = "ArtifactResolve";

        /// <summary>
        /// Name of artifact response
        /// </summary>
        public const string ArtifactResponse = "ArtifactResponse";
    }
}
