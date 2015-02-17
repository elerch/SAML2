using SAML2;
using System;
using System.Linq;
using System.Security.Claims;

namespace Owin.Security.Saml
{
    internal static class Saml20AssertionExtensions 
    {
        public static ClaimsIdentity ToClaimsIdentity(this Saml20Assertion value, string authenticationType, string nameType = null, string roleType = null)
        {
            if (value == null) throw new ArgumentNullException("value"); 
            return new ClaimsIdentity(value.Attributes.Select(a => a.ToClaim(value.Issuer)), authenticationType, nameType, roleType);
        }
    }
}
