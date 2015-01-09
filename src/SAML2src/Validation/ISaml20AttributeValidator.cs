using SAML2.Schema.Core;
using SAML2.Schema.Protocol;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Attributes Validator interface.
    /// </summary>
    public interface ISaml20AttributeValidator
    {
        /// <summary>
        /// Validates the attribute.
        /// </summary>
        /// <param name="samlAttribute">The SAML attribute.</param>
        void ValidateAttribute(SamlAttribute samlAttribute);

        /// <summary>
        /// Validates the encrypted attribute.
        /// </summary>
        /// <param name="encryptedElement">The encrypted element.</param>
        void ValidateEncryptedAttribute(EncryptedElement encryptedElement);
    }
}
