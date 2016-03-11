using SAML2.Schema.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace Owin.Security.Saml
{
    internal static class SamlAttributeExtensions
    {
        [Obsolete("This method joins the attribute value into a comma-delimeted string and should not be used. Use ToClaims instead")]
        public static Claim ToClaim(this SamlAttribute value, string issuer)
        {
            if (value == null) throw new ArgumentNullException("value");
            // TODO: Find the right answer to the multiple attributevalue question
            return new Claim(value.Name, string.Join(",", value.AttributeValue), value.NameFormat, issuer);
        }

        public static IEnumerable<Claim> ToClaims(this SamlAttribute value, string issuer)
        {
            if (value == null) throw new ArgumentNullException("value");
            return value.AttributeValue.Select(v => new Claim(value.Name, v, value.NameFormat, issuer));
        }
    }
}
