using System;
using SAML2.Schema.Protocol;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Encrypted Element validator.
    /// </summary>
    public class Saml20EncryptedElementValidator
    {
        /// <summary>
        /// Validates the encrypted element.
        /// </summary>
        /// <param name="encryptedElement">The encrypted element.</param>
        /// <param name="parentNodeName">Name of the parent node.</param>
        public void ValidateEncryptedElement(EncryptedElement encryptedElement, string parentNodeName)
        {
            if (encryptedElement == null)
            {
                throw new ArgumentNullException("encryptedElement");
            }

            if (encryptedElement.EncryptedData == null)
            {
                throw new Saml20FormatException(string.Format("An {0} MUST contain an xenc:EncryptedData element", parentNodeName));
            }

            if (encryptedElement.EncryptedData.Type != null
                && !string.IsNullOrEmpty(encryptedElement.EncryptedData.Type)
                && encryptedElement.EncryptedData.Type != Saml20Constants.Xenc + "Element")
            {
                throw new Saml20FormatException(string.Format("Type attribute of EncryptedData MUST have value {0} if it is present", Saml20Constants.Xenc + "Element"));
            }
        }
    }
}
