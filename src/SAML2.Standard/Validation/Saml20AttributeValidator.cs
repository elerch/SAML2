using System;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Attribute validator.
    /// </summary>
    public class Saml20AttributeValidator : ISaml20AttributeValidator
    {
        /// <summary>
        /// AnyAttributeValidator backing field.
        /// </summary>
        private readonly Saml20XmlAnyAttributeValidator _anyAttributeValidator = new Saml20XmlAnyAttributeValidator();

        /// <summary>
        /// EncryptedElementValidator backing field.
        /// </summary>
        private readonly Saml20EncryptedElementValidator _encryptedElementValidator = new Saml20EncryptedElementValidator();

        /// <summary>
        /// Validates the attribute.
        /// </summary>
        /// <remarks>
        /// [SAML2.0 standard] section 2.7.3.1
        /// </remarks>
        /// <param name="samlAttribute">The SAML attribute.</param>
        public void ValidateAttribute(SamlAttribute samlAttribute)
        {
            if (samlAttribute == null)
            {
                throw new ArgumentNullException("samlAttribute");
            }

            if (!Saml20Utils.ValidateRequiredString(samlAttribute.Name))
            {
                throw new Saml20FormatException("Name attribute of Attribute element MUST contain at least one non-whitespace character");
            }
            
            if (samlAttribute.AttributeValue != null)
            {
                foreach (object o in samlAttribute.AttributeValue)
                {
                    if (o == null)
                    {
                        throw new Saml20FormatException("null-AttributeValue elements are not supported");
                    }
                }
            }

            if (samlAttribute.AnyAttr != null)
            {
                _anyAttributeValidator.ValidateXmlAnyAttributes(samlAttribute.AnyAttr);
            }
        }

        /// <summary>
        /// Validates the encrypted attribute.
        /// </summary>
        /// <remarks>
        /// [SAML2.0 standard] section 2.7.3.2
        /// </remarks>
        /// <param name="encryptedElement">The encrypted element.</param>
        public void ValidateEncryptedAttribute(EncryptedElement encryptedElement)
        {
            _encryptedElementValidator.ValidateEncryptedElement(encryptedElement, "EncryptedAttribute");
        }
    }
}
