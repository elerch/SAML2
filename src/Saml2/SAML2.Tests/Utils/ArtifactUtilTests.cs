using System;
using NUnit.Framework;
using SAML2.Utils;

namespace SAML2.Tests.Utils
{
    /// <summary>
    /// <see cref="ArtifactUtil"/> tests.
    /// </summary>
    [TestFixture]
    public class ArtifactUtilTests
    {
        /// <summary>
        /// CreateArtifact method tests.
        /// </summary>
        [TestFixture]
        public class CreateArtifactMethod
        {
            /// <summary>
            /// Verify a created artifact can be parsed.
            /// </summary>
            [Test]
            public void CanParseCreatedArtifact()
            {
                // Arrange
                var sourceIdUrl = "https://kleopatra.safewhere.local/Saml2ExtWeb/artifact.ashx";

                var sourceIdHash = ArtifactUtil.GenerateSourceIdHash(sourceIdUrl);
                var messageHandle = ArtifactUtil.GenerateMessageHandle();

                short typeCode = 4;
                short endpointIndex = 1;

                // Act
                var artifact = ArtifactUtil.CreateArtifact(typeCode, endpointIndex, sourceIdHash, messageHandle);

                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[20];
                var parsedMessageHandle = new byte[20];

                var result = ArtifactUtil.TryParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);

                // Assert
                Assert.That(result, "Unable to parse artifact");
                Assert.That(typeCode == parsedTypeCode, "Original and parsed typeCode did not match");
                Assert.That(endpointIndex == parsedEndpointIndex, "Original and parsed endpointIndex did not match");

                for (var i = 0; i < 20; i++)
                {
                    if (sourceIdHash[i] != parsedSourceIdHash[i])
                    {
                        Assert.Fail("Original and parsed sourceIdHash are not identical");
                    }
                }

                for (var i = 0; i < 20; i++)
                {
                    if (messageHandle[i] != parsedMessageHandle[i])
                    {
                        Assert.Fail("Original and parsed messageHandle are not identical");
                    }
                }
            }

            /// <summary>
            /// Verify exception is thrown on message handle length mismatch.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsExceptionWhenMessageHandleLengthMismatch()
            {
                // Arrange
                short typeCode = 4;
                short endpointIndex = 1;
                var sourceIdHash = new byte[20];
                var messageHandle = new byte[19];

                // Act
                ArtifactUtil.CreateArtifact(typeCode, endpointIndex, sourceIdHash, messageHandle);
            }

            /// <summary>
            /// Verify exception is thrown on source id hash length mismatch.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsExceptionWhenSourceIdHashLengthMismatch()
            {
                // Arrange
                short typeCode = 4;
                short endpointIndex = 1;
                var sourceIdHash = new byte[19];
                var messageHandle = new byte[20];

                // Act
                ArtifactUtil.CreateArtifact(typeCode, endpointIndex, sourceIdHash, messageHandle);
            }
        }

        /// <summary>
        /// ParseArtifact method tests.
        /// </summary>
        [TestFixture]
        public class ParseArtifactMethod
        {
            /// <summary>
            /// Verify exception is thrown on source id hash length mismatch.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsExceptionWhenSourceIdHashLengthMismatch()
            {
                // Arrange
                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[19];
                var parsedMessageHandle = new byte[20];
                var artifact = string.Empty;

                // Act
                ArtifactUtil.ParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);
            }

            /// <summary>
            /// Verify exception is thrown on message handle length mismatch.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsExceptionWhenMessageHandleLengthMismatch()
            {
                // Arrange
                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[20];
                var parsedMessageHandle = new byte[19];
                var artifact = string.Empty;

                // Act
                ArtifactUtil.ParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);
            }

            /// <summary>
            /// Verify exception is thrown on artifact length mismatch.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsExceptionWhenArtifactLengthMismatch()
            {
                // Arrange
                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[20];
                var parsedMessageHandle = new byte[20];
                var artifact = string.Empty;

                // Act
                ArtifactUtil.ParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);
            }
        }

        /// <summary>
        /// TryParseArtifact method tests.
        /// </summary>
        [TestFixture]
        public class TryParseArtifact
        {
            /// <summary>
            /// Verify returns false on artifact length mismatch.
            /// </summary>
            [Test]
            public void ReturnsFalseOnArtifactLengthMismatch()
            {
                // Arrange
                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[20];
                var parsedMessageHandle = new byte[20];
                var artifact = string.Empty;

                // Act
                var result = ArtifactUtil.TryParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);

                // Assert
                Assert.That(!result, "TryParseArtifact did not fail as expected");
            }

            /// <summary>
            /// Verify returns false on source id hash length mismatch.
            /// </summary>
            [Test]
            public void ReturnsFalseOnSourceIdHashLengthMismatch()
            {
                // Arrange
                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[19];
                var parsedMessageHandle = new byte[20];
                var artifact = string.Empty;

                // Act
                var result = ArtifactUtil.TryParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);

                // Assert
                Assert.That(!result, "TryParseArtifact did not fail as expected");
            }
            
            /// <summary>
            /// Verify returns false on message handle length mismatch.
            /// </summary>
            [Test]
            public void ReturnsFalseOnMessageHandleLengthMismatch()
            {
                // Arrange
                short parsedTypeCode = -1;
                short parsedEndpointIndex = -1;
                var parsedSourceIdHash = new byte[20];
                var parsedMessageHandle = new byte[19];
                var artifact = string.Empty;

                // Act
                var result = ArtifactUtil.TryParseArtifact(artifact, ref parsedTypeCode, ref parsedEndpointIndex, ref parsedSourceIdHash, ref parsedMessageHandle);

                // Assert
                Assert.That(!result, "TryParseArtifact did not fail as expected");
            }
        }
    }
}
