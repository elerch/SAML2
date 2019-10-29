using System;

namespace SAML2
{
    /// <summary>
    /// Thrown when a token does not comply with the SAML 2.0 specification.
    /// </summary>
    public class Saml20FormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20FormatException"/> class.
        /// </summary>
        public Saml20FormatException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20FormatException"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public Saml20FormatException(string msg) : base(msg) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20FormatException"/> class.
        /// </summary>
        /// <param name="msg">A message describing the problem that caused the exception.</param>
        /// <param name="cause">Another exception that may be related to the problem.</param>
        public Saml20FormatException(string msg, Exception cause) : base(msg, cause) { }
    }
}
