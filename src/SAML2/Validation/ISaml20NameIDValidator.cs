using SAML2.Schema.Core;
using SAML2.Schema.Protocol;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 NameID validator interface.
    /// </summary>
    public interface ISaml20NameIdValidator
    {
        /// <summary>
        /// Validates the name ID.
        /// </summary>
        /// <param name="nameId">The name ID.</param>
        void ValidateNameId(NameId nameId);

        /// <summary>
        /// Validates the encrypted ID.
        /// </summary>
        /// <param name="encryptedId">The encrypted ID.</param>
        void ValidateEncryptedId(EncryptedElement encryptedId);
    }
}
