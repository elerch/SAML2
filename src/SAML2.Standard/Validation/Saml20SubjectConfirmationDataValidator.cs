using System;
using SAML2.Schema.Core;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 SubjectConfirmationData validator.
    /// </summary>
    public class Saml20SubjectConfirmationDataValidator : ISaml20SubjectConfirmationDataValidator
    {
        /// <summary>
        /// The <c>AnyAttr</c>c> validator.
        /// </summary>
        private readonly Saml20XmlAnyAttributeValidator _anyAttrValidator = new Saml20XmlAnyAttributeValidator();

        /// <summary>
        /// The <c>KeyInfo</c> validator.
        /// </summary>
        private readonly Saml20KeyInfoValidator _keyInfoValidator = new Saml20KeyInfoValidator();

        #region ISaml20SubjectConfirmationDataValidator Members

        /// <summary>
        /// Validate SubjectConfirmationData.
        /// </summary>
        /// <param name="subjectConfirmationData">The subject confirmation data.</param>
        /// <remarks>
        /// [SAML2.0 standard] section 2.4.1.2
        /// </remarks>
        public void ValidateSubjectConfirmationData(SubjectConfirmationData subjectConfirmationData)
        {
            // If present it must be anyUri
            if (subjectConfirmationData.Recipient != null)
            {
                if (!Uri.IsWellFormedUriString(subjectConfirmationData.Recipient, UriKind.Absolute))
                {
                    throw new Saml20FormatException("Recipient of SubjectConfirmationData must be a wellformed absolute URI.");
                }
            }

            // NotBefore MUST BE striclty less than NotOnOrAfter if they are both set
            if (subjectConfirmationData.NotBefore != null && subjectConfirmationData.NotOnOrAfter != null && !(subjectConfirmationData.NotBefore < subjectConfirmationData.NotOnOrAfter))
            {
                throw new Saml20FormatException(string.Format("NotBefore {0} MUST BE less than NotOnOrAfter {1} on SubjectConfirmationData", Saml20Utils.ToUtcString(subjectConfirmationData.NotBefore.Value), Saml20Utils.ToUtcString(subjectConfirmationData.NotOnOrAfter.Value)));
            }

            // Make sure the extension-attributes are namespace-qualified and do not use reserved namespaces
            if (subjectConfirmationData.AnyAttr != null)
            {
                _anyAttrValidator.ValidateXmlAnyAttributes(subjectConfirmationData.AnyAttr);
            }

            // Standards-defined extension type which has stricter rules than it's base type
            if (subjectConfirmationData is KeyInfoConfirmationData)
            {
                _keyInfoValidator.ValidateKeyInfo(subjectConfirmationData);
            }
        }

        #endregion
    }
}
