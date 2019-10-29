using System;

namespace SAML2.Config
{
    /// <summary>
    /// Binding types.
    /// </summary>
    [Flags]
    public enum BindingType
    {
        /// <summary>
        /// No binding set.
        /// </summary>
        NotSet = 0,

        /// <summary>
        /// POST binding
        /// </summary>
        Post = 1,

        /// <summary>
        /// Redirect binding
        /// </summary>
        Redirect = 2,

        /// <summary>
        /// Artifact binding
        /// </summary>
        Artifact = 4,

        /// <summary>
        /// SOAP binding
        /// </summary>
        Soap = 8
    }
}
