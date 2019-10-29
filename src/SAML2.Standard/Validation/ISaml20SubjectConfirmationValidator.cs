using SAML2.Schema.Core;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Subject Confirmation Validator interface.
    /// </summary>
    public interface ISaml20SubjectConfirmationValidator
    {
        /// <summary>
        /// Validates the subject confirmation.
        /// </summary>
        /// <param name="subjectConfirmation">The subject confirmation.</param>
        void ValidateSubjectConfirmation(SubjectConfirmation subjectConfirmation);
    }
}