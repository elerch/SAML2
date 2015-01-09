using System;
using SAML2.Schema.Core;
using SAML2.Schema.XmlDSig;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 KeyInfo validator.
    /// </summary>
    public class Saml20KeyInfoValidator
    {
        /// <summary>
        /// Validates the presence and correctness of a <ds:KeyInfo xmlns:ds="http://www.w3.org/2000/09/xmldsig#"/> among the any-xml-elements of a SubjectConfirmationData
        /// </summary>
        /// <param name="subjectConfirmationData">The subject confirmation data.</param>
        public void ValidateKeyInfo(SubjectConfirmationData subjectConfirmationData)
        {
            if (subjectConfirmationData == null)
            {
                throw new Saml20FormatException("SubjectConfirmationData cannot be null when KeyInfo subelements are required");
            }

            if (subjectConfirmationData.AnyElements == null)
            {
                throw new Saml20FormatException(string.Format("SubjectConfirmationData element MUST have at least one {0} subelement", KeyInfo.ElementName));
            }

            var keyInfoFound = false;
            foreach (var element in subjectConfirmationData.AnyElements)
            {
                if (element.NamespaceURI != Saml20Constants.Xmldsig || element.LocalName != KeyInfo.ElementName)
                {
                    continue;
                }

                keyInfoFound = true;

                // A KeyInfo element MUST identify a cryptographic key
                if (!element.HasChildNodes)
                {
                    throw new Saml20FormatException(string.Format("{0} subelement of SubjectConfirmationData MUST NOT be empty", KeyInfo.ElementName));
                }
            }

            // There MUST BE at least one <ds:KeyInfo> element present (from the arbitrary elements list on SubjectConfirmationData
            if (!keyInfoFound)
            {
                throw new Saml20FormatException(string.Format("SubjectConfirmationData element MUST contain at least one {0} in namespace {1}", KeyInfo.ElementName, Saml20Constants.Xmldsig));
            }
        }
    }
}
