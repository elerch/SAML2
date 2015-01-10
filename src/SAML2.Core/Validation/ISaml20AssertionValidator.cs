using System;
using SAML2.Schema.Core;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Assertion Validator interface.
    /// </summary>
    public interface ISaml20AssertionValidator
    {
        /// <summary>
        /// Validates the assertion.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        void ValidateAssertion(Assertion assertion);

        /// <summary>
        /// Validates the time restrictions.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="allowedClockSkew">The allowed clock skew.</param>
        void ValidateTimeRestrictions(Assertion assertion, TimeSpan allowedClockSkew);
    }
}
