using System;
using System.Xml;

namespace SAML2.Utils
{
    /// <summary>
    /// Helpers for converting between string and DateTime representations of UTC date-times
    /// and for enforcing the UTC-string-format demand for xml strings in SAML2.0
    /// </summary>
    public static class Saml20Utils
    {
        /// <summary>
        /// Converts from the UTC string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The represented DateTime.</returns>
        public static DateTime FromUtcString(string value)
        {
            try
            {
                return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
            }
            catch (FormatException fe)
            {
                throw new Saml20FormatException("Invalid DateTime-string (non-UTC) found in saml:Assertion", fe);
            }
        }

        /// <summary>
        /// Toes the UTC string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The DateTime represented as a UTC string.</returns>
        public static string ToUtcString(DateTime value)
        {
            return XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// Make sure that the ID elements is at least 128 bits in length (SAML2.0 standard section 1.3.4)
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>True if the id is valid, else false.</returns>
        public static bool ValidateIdString(string id)
        {
            return id != null && id.Length >= 16;
        }

        /// <summary>
        /// A string value marked as OPTIONAL in [SAML2.0 standard] must contain at least one non-whitespace character
        /// </summary>
        /// <param name="optString">The opt string.</param>
        /// <returns>True if the string is value, else false.</returns>
        public static bool ValidateOptionalString(string optString)
        {
            return optString == null || ValidateRequiredString(optString);
        }

        /// <summary>
        /// A string value marked as REQUIRED in [SAML2.0 standard] must contain at least one non-whitespace character
        /// </summary>
        /// <param name="reqString">The required string.</param>
        /// <returns>True if the string is value, else false.</returns>
        public static bool ValidateRequiredString(string reqString)
        {
            return !(string.IsNullOrEmpty(reqString) || reqString.Trim().Length == 0);
        }
    }
}
