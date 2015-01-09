using NUnit.Framework;
using SAML2.Schema.Core;
using SAML2.Validation;

namespace SAML2.Tests.Validation
{
    /// <summary>
    /// <see cref="Saml20NameIdValidator"/> tests.
    /// </summary>
    [TestFixture]
    public class Saml20NameIdValidatorTests
    {
        /// <summary>
        /// ValidateNameId method tests.
        /// </summary>
        [TestFixture]
        public class ValidateNameIdMethod
        {
            #region Email

            /// <summary>
            /// Tests various invalid email addresses. The validation uses the .NET class MailAddress for validation
            /// which explains the large number of tested addresses
            /// </summary>
            [Test]
            public void ThrowsExceptionWhenEmailInvalidForm()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Email
                                 };
                var validator = new Saml20NameIdValidator();

                var invalidEmails = new[]
                                        {
                                            "thisisnotavalid.email@ ",
                                            "thisisnotavalidemail",
                                            "thisisnotavalidemail.com",
                                            "@thisisnotavalidemail.com",
                                            " @thisisnotavalidemail.com",
                                            "@ @thisisnotavalidemail.com",
                                            " @ @thisisnotavalidemail.com",
                                            " . @thisisnotavalidemail.com",
                                            @"\. @thisisnotavalidemail.com",
                                            @"\.\@thisisnotavalidemail.com",
                                            @"a.\@thisisnotavalidemail.com",
                                            @"<.>@thisisnotavalidemail.com",
                                            @"<.a@thisisnotavalidemail.com",
                                            "thisisnotavalid.email@",
                                            "thisisnotavalid.email@ @",
                                            "thisisnotavalid.email@ @ "
                                        };

                foreach (var email in invalidEmails)
                {
                    nameId.Value = email;

                    try
                    {
                        // Act
                        validator.ValidateNameId(nameId);

                        // Assert
                        Assert.Fail("Email address " + email + " is not supposed to be valid");
                    }
                    catch (Saml20FormatException sfe)
                    {
                        Assert.AreEqual(sfe.Message, "Value of NameID is not a valid email address according to the IETF RFC 2822 specification");
                    }
                }
            }

            /// <summary>
            /// Verify exception is thrown on Email Value containing only whitespace characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Email Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenEmailValueContainsOnlyWhitespace()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Email,
                                     Value = " "
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify validates valid email.
            /// </summary>
            [Test]
            public void ValidatesEmail()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Email,
                                     Value = "my.address@yours.com"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            #endregion

            #region X509SubjectName

            /// <summary>
            /// Verify exception is thrown on X509SubjectName Value containing only whitespace characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with X509SubjectName Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenX509SubjecNameValueContainsOnlyWhirespace()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.X509SubjectName,
                                     Value = " "
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on X509SubjectName Value being empty.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with X509SubjectName Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenX509SubjecNameValueEmpty()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.X509SubjectName,
                                     Value = string.Empty
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            #endregion

            #region Windows

            /// <summary>
            /// Verify exception is thrown on WindowsDomainQualifiedName Value containing only whitespace characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Windows Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenWindowsDomainQualifiedNameValueContainsOnlyWhitespace()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Windows,
                                     Value = " "
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify validates WindowsDomainQualifiedName.
            /// </summary>
            [Test]
            public void ValidatesWindowsDomainQualifiedName()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Windows
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                nameId.Value = "a";
                validator.ValidateNameId(nameId);

                nameId.Value = "b\a";
                validator.ValidateNameId(nameId);
            }

            #endregion

            #region Kerberos

            /// <summary>
            /// Verify exception is thrown on Kerberos Value containing only whitespace characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Kerberos Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenKerberosValueContainsOnlyWhitespace()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Kerberos,
                                     Value = " "
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Kerberos Value being empty.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Kerberos Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenKerberosValueEmpty()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Kerberos,
                                     Value = string.Empty
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on kerberos invalid format.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Kerberos Format attribute MUST contain a Value that contains a '@'")]
            public void ThrowsExceptionWhenKerberosInvalidFormat()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Kerberos,
                                     Value = @"a\b"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Kerberos Value with length less than three characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Kerberos Format attribute MUST contain a Value with at least 3 characters")]
            public void ThrowsExceptionWhenKerberosLessThanThreecharacters()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Kerberos,
                                     Value = @"b"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify validates kerberos.
            /// </summary>
            [Test]
            public void ValidatesKerberos()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Kerberos,
                                     Value = "a@b"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            #endregion

            #region Entity

            /// <summary>
            /// Verify exception is thrown on Entity Value containing only whitespace characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Entity Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenEntityValueContainsOnlyWhitespace()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = " "
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Entity Value being empty.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Entity Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenEntityValueEmpty()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = string.Empty
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Entity Value length being too long.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Entity Format attribute MUST have a Value that contains no more than 1024 characters")]
            public void ThrowsExceptionWhenEntityLengthTooLong()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = new string('f', 1025)
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Entity with NameQualifier set.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Entity Format attribute MUST NOT set the NameQualifier attribute")]
            public void ThrowsExceptionWhenEntityNameQualifierSet()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = new string('f', 1024),
                                     NameQualifier = "ksljdf"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Entity SPNameQualifier set.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Entity Format attribute MUST NOT set the SPNameQualifier attribute")]
            public void ThrowsExceptionWhenEntitySPNameQualifierSet()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = new string('f', 1024),
                                     SPNameQualifier = "ksljdf"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Entity SPProvidedID set.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Entity Format attribute MUST NOT set the SPProvidedID attribute")]
            public void ThrowsExceptionWhenEntitySPProvidedId()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = new string('f', 1024),
                                     SPProvidedID = "ksljdf"
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify entity can be validated.
            /// </summary>
            [Test]
            public void ValidatesEntity()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Entity,
                                     Value = new string('f', 1024)
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            #endregion

            #region Persistent

            /// <summary>
            /// Verify exception is thrown on Persistent Value containing only whitespace characters.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Persistent Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenPersistentContainsOnlyWhitespace()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Persistent,
                                     Value = " "
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Persistent Value being empty.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Persistent Format attribute MUST contain a Value that contains more than whitespace characters")]
            public void ThrowsExceptionWhenPersistentValueEmpty()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Persistent,
                                     Value = string.Empty
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Persistent Value length being too long.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Persistent Format attribute MUST have a Value that contains no more than 256 characters")]
            public void ThrowsExceptionWhenPersistentLengthTooLong()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Persistent,
                                     Value = new string('f', 257)
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify validates persistent.
            /// </summary>
            [Test]
            public void ValidatesPersistent()
            {
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Persistent,
                                     Value = new string('f', 256)
                                 };
                var validator = new Saml20NameIdValidator();
                validator.ValidateNameId(nameId);
            }

            #endregion

            #region Transient

            /// <summary>
            /// Verify exception is thrown on Transient Value length being too long.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Transient Format attribute MUST have a Value that contains no more than 256 characters")]
            public void ThrowsExceptionWhenTransientValueTooLong()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Transient,
                                     Value = new string('f', 257)
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }

            /// <summary>
            /// Verify exception is thrown on Transient Value being too short.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NameID with Transient Format attribute MUST have a Value with at least 16 characters (the equivalent of 128 bits)")]
            public void ThrowsExceptionWhenTransientValueTooShort()
            {
                // Arrange
                var nameId = new NameId
                                 {
                                     Format = Saml20Constants.NameIdentifierFormats.Transient,
                                     Value = new string('f', 15)
                                 };
                var validator = new Saml20NameIdValidator();

                // Act
                validator.ValidateNameId(nameId);
            }
            
            /// <summary>
            /// Verify validates transient.
            /// </summary>
            [Test]
            public void ValidatesTransient()
            {
                var nameId = new NameId { Format = Saml20Constants.NameIdentifierFormats.Transient };
                var validator = new Saml20NameIdValidator();

                nameId.Value = new string('f', 256);
                validator.ValidateNameId(nameId);

                nameId.Value = new string('f', 16);
                validator.ValidateNameId(nameId);
            }

            #endregion
        }
    }
}
