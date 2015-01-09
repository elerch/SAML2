using System;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 SubjectConfirmation validator.
    /// </summary>
    public class Saml20SubjectConfirmationValidator : ISaml20SubjectConfirmationValidator
    {
        /// <summary>
        /// KeyInfo validator.
        /// </summary>
        private readonly Saml20KeyInfoValidator _keyInfoValidator = new Saml20KeyInfoValidator();

        /// <summary>
        /// NameID validator.
        /// </summary>
        private readonly ISaml20NameIdValidator _nameIdValidator = new Saml20NameIdValidator();

        /// <summary>
        /// SubjectConfirmationData validator.
        /// </summary>
        private readonly ISaml20SubjectConfirmationDataValidator _subjectConfirmationDataValidator = new Saml20SubjectConfirmationDataValidator();

        /// <summary>
        /// Validates the subject confirmation.
        /// </summary>
        /// <param name="subjectConfirmation">The subject confirmation.</param>
        public void ValidateSubjectConfirmation(SubjectConfirmation subjectConfirmation)
        {
            if (subjectConfirmation == null)
            {
                throw new ArgumentNullException("subjectConfirmation");
            }

            if (!Saml20Utils.ValidateRequiredString(subjectConfirmation.Method))
            {
                throw new Saml20FormatException("Method attribute of SubjectConfirmation MUST contain at least one non-whitespace character");
            }

            if (!Uri.IsWellFormedUriString(subjectConfirmation.Method, UriKind.Absolute))
            {
                throw new Saml20FormatException("SubjectConfirmation element has Method attribute which is not a wellformed absolute uri.");
            }

            if (subjectConfirmation.Method == Saml20Constants.SubjectConfirmationMethods.HolderOfKey)
            {
                _keyInfoValidator.ValidateKeyInfo(subjectConfirmation.SubjectConfirmationData);
            }

            if (subjectConfirmation.Item != null)
            {
                if (subjectConfirmation.Item is NameId)
                {
                    _nameIdValidator.ValidateNameId((NameId)subjectConfirmation.Item);
                }
                else if (subjectConfirmation.Item is EncryptedElement)
                {
                    _nameIdValidator.ValidateEncryptedId((EncryptedElement)subjectConfirmation.Item);
                }
                else
                {
                    throw new Saml20FormatException(string.Format("Identifier of type {0} is not supported for SubjectConfirmation", subjectConfirmation.Item.GetType()));
                }
            }
            else if (subjectConfirmation.SubjectConfirmationData != null)
            {
                _subjectConfirmationDataValidator.ValidateSubjectConfirmationData(subjectConfirmation.SubjectConfirmationData);
            }
        }
    }
}
