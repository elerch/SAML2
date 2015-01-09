using SAML2.Schema.Core;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Subject Confirmation Data Validator interface.
    /// </summary>
    public interface ISaml20SubjectConfirmationDataValidator
    {
        /// <summary>
        /// Validates the subject confirmation data.
        /// </summary>
        /// <param name="data">The data.</param>
        void ValidateSubjectConfirmationData(SubjectConfirmationData data);
    }
}
