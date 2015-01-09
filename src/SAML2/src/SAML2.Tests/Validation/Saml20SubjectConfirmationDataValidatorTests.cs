using System;
using System.Xml;
using NUnit.Framework;
using SAML2.Schema.Core;
using SAML2.Schema.XmlDSig;
using SAML2.Validation;

namespace SAML2.Tests.Validation
{
    /// <summary>
    /// <see cref="Saml20SubjectConfirmationDataValidator"/> tests.
    /// </summary>
    [TestFixture]
    public class Saml20SubjectConfirmationDataValidatorTests
    {
        /// <summary>
        /// ValidateSubjectConfirmationData method tests.
        /// </summary>
        [TestFixture]
        public class ValidateSubjectConfirmationDataMethod
        {
            /// <summary>
            /// Verify exception is thrown when key info confirmation data has no any elements.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "SubjectConfirmationData element MUST have at least one " + KeyInfo.ElementName + " subelement")]
            public void ThrowsExceptionWhenKeyInfoConfirmationDataHasNoElements()
            {
                // Arrange
                var subjectConfirmationData = new KeyInfoConfirmationData { Recipient = "urn:wellformed.uri:ok" };
                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Verify exception is thrown when key info confirmation data has no elements with correct namespace.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "SubjectConfirmationData element MUST contain at least one " + KeyInfo.ElementName + " in namespace " + Saml20Constants.Xmldsig)]
            public void ThrowsExceptionWhenKeyInfoConfirmationDataHasNoElementsWithCorrectNamespace()
            {
                // Arrange
                var subjectConfirmationData = new KeyInfoConfirmationData();
                subjectConfirmationData.Recipient = "urn:wellformed.uri:ok";
                var doc = new XmlDocument();
                subjectConfirmationData.AnyElements = new[] { doc.CreateElement("ds", "KeyInfo", "http://wrongNameSpace.uri") };

                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Verify exception is thrown when key info confirmation data has no elements with valid key.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "SubjectConfirmationData element MUST contain at least one " + KeyInfo.ElementName + " in namespace " + Saml20Constants.Xmldsig)]
            public void ThrowsExceptionWhenKeyInfoConfirmationDataHasNoElementsWithValidKeyName()
            {
                // Arrange
                var subjectConfirmationData = new KeyInfoConfirmationData { Recipient = "urn:wellformed.uri:ok" };
                var doc = new XmlDocument();
                var elem = doc.CreateElement("ds", "KeyInfo", "http://wrongNameSpace.uri");
                elem.AppendChild(doc.CreateElement("ds", "KeyName", Saml20Constants.Xmldsig));

                subjectConfirmationData.AnyElements = new[] { elem };

                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Verify exception is thrown when key info confirmation data sub element has no children.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "KeyInfo subelement of SubjectConfirmationData MUST NOT be empty")]
            public void ThrowsExceptionWhenKeyInfoConfirmationDataSubElementHasNoChildren()
            {
                // Arrange
                var subjectConfirmationData = new KeyInfoConfirmationData { Recipient = "urn:wellformed.uri:ok" };
                var doc = new XmlDocument();
                subjectConfirmationData.AnyElements = new[] { doc.CreateElement("ds", "KeyInfo", Saml20Constants.Xmldsig) };

                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Tests the validation of the SubjectConfirmationData recipient element
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "Recipient of SubjectConfirmationData must be a wellformed absolute URI.")]
            public void ThrowsExceptionWhenSubjectConfirmationDataRecipientIsEmpty()
            {
                // Arrange
                var subjectConfirmationData = new SubjectConfirmationData { Recipient = " " };
                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Tests the validation of the SubjectConfirmationData recipient element
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "Recipient of SubjectConfirmationData must be a wellformed absolute URI.")]
            public void ThrowsExceptionWhenSubjectConfirmationDataRecipientIsInvalid()
            {
                // Arrange
                var subjectConfirmationData = new SubjectConfirmationData { Recipient = "malformed uri" };
                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Tests the validation of the SubjectConfirmationData {NotBefore, NotOnOrAfter} attributes
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException), ExpectedMessage = "NotBefore 2008-01-30T17:13:00.5Z MUST BE less than NotOnOrAfter 2008-01-30T16:13:00.5Z on SubjectConfirmationData")]
            public void ThrowsExceptionWhenSubjectConfirmationDataTimeIntervalIsInvalid()
            {
                // Arrange
                var subjectConfirmationData = new SubjectConfirmationData();
                subjectConfirmationData.NotBefore = new DateTime(2008, 01, 30, 17, 13, 0, 500, DateTimeKind.Utc);
                subjectConfirmationData.NotOnOrAfter = subjectConfirmationData.NotBefore.Value.AddHours(-1);

                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Verify validates the key info confirmation data.
            /// </summary>
            [Test]
            public void ValidatesKeyInfoConfirmationData()
            {
                // Arrange
                var subjectConfirmationData = new KeyInfoConfirmationData { Recipient = "urn:wellformed.uri:ok" };
                var doc = new XmlDocument();
                var elem = doc.CreateElement("ds", "KeyInfo", Saml20Constants.Xmldsig);
                elem.AppendChild(doc.CreateElement("lalala"));

                subjectConfirmationData.AnyElements = new[] { elem };

                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Tests the validation of the SubjectConfirmationData recipient element
            /// </summary>
            [Test]
            public void ValidatesSubjectConfirmationDataRecipient()
            {
                // Arrange
                var subjectConfirmationData = new SubjectConfirmationData { Recipient = "urn:wellformed.uri:ok" };
                var validator = new Saml20SubjectConfirmationDataValidator();

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }

            /// <summary>
            /// Tests the validation of the SubjectConfirmationData {NotBefore, NotOnOrAfter} attributes
            /// </summary>
            [Test]
            public void ValidatesSubjectConfirmationDataTimeIntervalSettings()
            {
                // TODO: Split this up
                // Arrange
                var validator = new Saml20SubjectConfirmationDataValidator();

                var subjectConfirmationData = new SubjectConfirmationData();
                subjectConfirmationData.NotBefore = new DateTime(2008, 01, 30, 17, 13, 0, 500, DateTimeKind.Utc);
                subjectConfirmationData.NotOnOrAfter = subjectConfirmationData.NotBefore.Value.AddHours(1);

                validator.ValidateSubjectConfirmationData(subjectConfirmationData);

                subjectConfirmationData.NotBefore = null;
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);

                // DateTime validation wrt DateTime.UtcNow is NOT done by the validators
                // so a future-NotBefore must be valid
                subjectConfirmationData.NotBefore = subjectConfirmationData.NotOnOrAfter;
                subjectConfirmationData.NotOnOrAfter = null;
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);

                subjectConfirmationData.NotBefore = null;

                // Act
                validator.ValidateSubjectConfirmationData(subjectConfirmationData);
            }
        }
    }
}
