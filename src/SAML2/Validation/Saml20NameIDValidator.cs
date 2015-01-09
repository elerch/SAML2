using System;
using System.Net.Mail;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 NameID validator.
    /// </summary>
    public class Saml20NameIdValidator : ISaml20NameIdValidator
    {
        /// <summary>
        /// The encrypted element validator.
        /// </summary>
        private readonly Saml20EncryptedElementValidator _encryptedElementValidator = new Saml20EncryptedElementValidator();

        /// <summary>
        /// Validates the name ID.
        /// </summary>
        /// <param name="nameId">The name ID.</param>
        public void ValidateNameId(NameId nameId)
        {
            if (nameId == null)
            {
                throw new ArgumentNullException("nameId");
            }

            if (string.IsNullOrEmpty(nameId.Format))
            {
                return;
            }

            if (!Uri.IsWellFormedUriString(nameId.Format, UriKind.Absolute))
            {
                throw new Saml20FormatException("NameID element has Format attribute which is not a wellformed absolute uri.");
            }

            // The processing rules from [SAML2.0 standard] section 8.3 are implemented here
            if (nameId.Format == Saml20Constants.NameIdentifierFormats.Email)
            {
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Email Format attribute MUST contain a Value that contains more than whitespace characters");
                }

                try
                {
                    new MailAddress(nameId.Value);
                }
                catch (FormatException fe)
                {
                    throw new Saml20FormatException("Value of NameID is not a valid email address according to the IETF RFC 2822 specification", fe);
                }
                catch (IndexOutOfRangeException ie)
                {
                    throw new Saml20FormatException("Value of NameID is not a valid email address according to the IETF RFC 2822 specification", ie);
                }
            }
            else if (nameId.Format == Saml20Constants.NameIdentifierFormats.X509SubjectName)
            {
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with X509SubjectName Format attribute MUST contain a Value that contains more than whitespace characters");
                }

                // TODO: Consider checking for correct encoding of the Value according to the
                // XML Signature Recommendation (http://www.w3.org/TR/xmldsig-core/) section 4.4.4
            }
            else if (nameId.Format == Saml20Constants.NameIdentifierFormats.Windows)
            {
                // Required format is 'DomainName\UserName' but the domain name and the '\' are optional
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Windows Format attribute MUST contain a Value that contains more than whitespace characters");
                }
            }
            else if (nameId.Format == Saml20Constants.NameIdentifierFormats.Kerberos)
            {
                // Required format is 'name[/instance]@REALM'
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Kerberos Format attribute MUST contain a Value that contains more than whitespace characters");
                }

                if (nameId.Value.Length < 3)
                {
                    throw new Saml20FormatException("NameID with Kerberos Format attribute MUST contain a Value with at least 3 characters");
                }

                if (nameId.Value.IndexOf("@") < 0)
                {
                    throw new Saml20FormatException("NameID with Kerberos Format attribute MUST contain a Value that contains a '@'");
                }

                // TODO: Consider implementing the rules for 'name', 'instance' and 'REALM' found in IETF RFC 1510 (http://www.ietf.org/rfc/rfc1510.txt) here 
            }
            else if (nameId.Format == Saml20Constants.NameIdentifierFormats.Entity)
            {
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Entity Format attribute MUST contain a Value that contains more than whitespace characters");
                }

                if (nameId.Value.Length > 1024)
                {
                    throw new Saml20FormatException("NameID with Entity Format attribute MUST have a Value that contains no more than 1024 characters");
                }

                if (nameId.NameQualifier != null)
                {
                    throw new Saml20FormatException("NameID with Entity Format attribute MUST NOT set the NameQualifier attribute");
                }
                
                if (nameId.SPNameQualifier != null)
                {
                    throw new Saml20FormatException("NameID with Entity Format attribute MUST NOT set the SPNameQualifier attribute");
                }
                
                if (nameId.SPProvidedID != null)
                {
                    throw new Saml20FormatException("NameID with Entity Format attribute MUST NOT set the SPProvidedID attribute");
                }
            }
            else if (nameId.Format == Saml20Constants.NameIdentifierFormats.Persistent)
            {
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Persistent Format attribute MUST contain a Value that contains more than whitespace characters");
                }

                if (nameId.Value.Length > 256)
                {
                    throw new Saml20FormatException("NameID with Persistent Format attribute MUST have a Value that contains no more than 256 characters");
                }
            }
            else if (nameId.Format == Saml20Constants.NameIdentifierFormats.Transient)
            {
                if (!Saml20Utils.ValidateRequiredString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Transient Format attribute MUST contain a Value that contains more than whitespace characters");
                }

                if (nameId.Value.Length > 256)
                {
                    throw new Saml20FormatException("NameID with Transient Format attribute MUST have a Value that contains no more than 256 characters");
                }

                if (!Saml20Utils.ValidateIdString(nameId.Value))
                {
                    throw new Saml20FormatException("NameID with Transient Format attribute MUST have a Value with at least 16 characters (the equivalent of 128 bits)");
                }
            }
        }

        /// <summary>
        /// Validates the encrypted ID.
        /// </summary>
        /// <param name="encryptedId">The encrypted ID.</param>
        public void ValidateEncryptedId(EncryptedElement encryptedId)
        {
            _encryptedElementValidator.ValidateEncryptedElement(encryptedId, "EncryptedID");
        }
    }
}
