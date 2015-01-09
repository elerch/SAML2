using System.Security.Principal;
using System.Web;

namespace SAML2.Identity
{
    /// <summary>
    /// The Principal cache for SAML2.
    /// </summary>
    internal class Saml20PrincipalCache
    {
        /// <summary>
        /// Adds the principal.
        /// </summary>
        /// <param name="principal">The principal.</param>
        internal static void AddPrincipal(IPrincipal principal)
        {
            if (HttpContext.Current.Session != null) 
            {
                HttpContext.Current.Session[typeof(Saml20Identity).FullName] = principal;
            }
            HttpContext.Current.Items[typeof(Saml20Identity).FullName] = principal;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal static void Clear()
        {
            if (HttpContext.Current.Session != null) 
            {
                HttpContext.Current.Session.Remove(typeof(Saml20Identity).FullName);
            }
            HttpContext.Current.Items.Remove(typeof(Saml20Identity).FullName);
        }

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <returns>The <see cref="IPrincipal"/>.</returns>
        internal static IPrincipal GetPrincipal()
        {
            return HttpContext.Current.Session != null ? HttpContext.Current.Session[typeof(Saml20Identity).FullName] as GenericPrincipal : HttpContext.Current.Items[typeof(Saml20Identity).FullName] as GenericPrincipal;
        }
    }
}
