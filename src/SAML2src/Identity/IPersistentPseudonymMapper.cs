using SAML2.Schema.Core;

namespace SAML2.Identity
{
    /// <summary>
    /// Implement this interface and register it in web.config on the relevant IdP endpoint to activate mapping of persistent pseudonyms
    /// before creating the name of the current IPrincipal. The returned value will be set as the Name on the Identity of the current Principal.
    /// If not registered, the Identity.Name value will be the value of the SAML Subject as returned by the IdP. 
    /// </summary>
    public interface IPersistentPseudonymMapper
    {
        /// <summary>
        /// Service-provider specific pseudonym mapping is implemented here
        /// </summary>
        /// <param name="samlSubject">The SAML Subject identity value returned by the IdP</param>
        /// <returns>The service-provider specific mapping of the input parameter</returns>
        string MapIdentity(NameId samlSubject);
    }
}
