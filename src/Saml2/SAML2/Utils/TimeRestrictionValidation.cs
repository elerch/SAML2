using System;

namespace SAML2.Utils
{
    /// <summary>
    /// Utility functions for validating SAML message time stamps
    /// </summary>
    public static class TimeRestrictionValidation
    {
        /// <summary>
        /// Handle allowed clock skew by decreasing notBefore with allowedClockSkew
        /// </summary>
        /// <param name="notBefore">The not before.</param>
        /// <param name="now">The now.</param>
        /// <param name="allowedClockSkew">The allowed clock skew.</param>
        /// <returns>True if the not before value is valid, else false.</returns>
        public static bool NotBeforeValid(DateTime notBefore, DateTime now, TimeSpan allowedClockSkew)
        {
            return notBefore.Subtract(allowedClockSkew) <= now;
        }

        /// <summary>
        /// Handle allowed clock skew by increasing notOnOrAfter with allowedClockSkew
        /// </summary>
        /// <param name="notOnOrAfter">The not on or after.</param>
        /// <param name="now">The now.</param>
        /// <param name="allowedClockSkew">The allowed clock skew.</param>
        /// <returns>True if the not on or after value is valid, else false.</returns>
        public static bool NotOnOrAfterValid(DateTime notOnOrAfter, DateTime now, TimeSpan allowedClockSkew)
        {
            return now < notOnOrAfter.Add(allowedClockSkew);
        }
    }
}