using SAML2.Schema.Core;
using System;
using System.Security.Claims;

namespace Owin.Security.Saml
{
    internal static class SamlAttributeExtensions
    {
        public static Claim ToClaim(this SamlAttribute value, string issuer)
        {
            if (value == null) throw new ArgumentNullException("value");
            // TODO: Find the right answer to the multiple attributevalue question
            return new Claim(value.Name, string.Join(",", value.AttributeValue), value.NameFormat, issuer);
        }
    }
}
