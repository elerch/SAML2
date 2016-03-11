using SAML2;
using System;
using System.Linq;
using System.Security.Claims;
using SAML2.Schema.Core;
using System.Collections.Generic;

namespace Owin.Security.Saml
{
    internal static class Saml20AssertionExtensions 
    {
        public static ClaimsIdentity ToClaimsIdentity(this Saml20Assertion value, string authenticationType, string nameType = null, string roleType = null)
        {
            if (value == null) throw new ArgumentNullException("value"); 
            return new ClaimsIdentity(value.Attributes.SelectMany(a => a.ToClaims(value.Issuer)).Concat(ClaimsFromSubject(value.Subject, value.Issuer)), authenticationType, nameType, roleType);
        }

        private static IEnumerable<Claim> ClaimsFromSubject(NameId subject, string issuer)
        {
            if (subject == null) yield break;
            if (subject.Value != null) {
                yield return new Claim("sub", subject.Value, ClaimValueTypes.String, issuer); // openid connect
                yield return new Claim(ClaimTypes.NameIdentifier, subject.Value, ClaimValueTypes.String, issuer); // saml
            }
        }
    }
}
