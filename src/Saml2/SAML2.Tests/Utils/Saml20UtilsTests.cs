using System;
using NUnit.Framework;
using SAML2.Utils;

namespace SAML2.Tests.Utils
{
    /// <summary>
    /// <see cref="Saml20Utils"/>  tests.
    /// </summary>
    [TestFixture]
    public class Saml20UtilsTests
    {
        /// <summary>
        /// <c>FromUtcString</c> method tests.
        /// </summary>
        [TestFixture]
        public class FromUtcStringMethod
        {
            /// <summary>
            /// Verify can convert UTC formatted string.
            /// </summary>
            [Test]
            public void CanConvertString()
            {
                // Arrange
                var now = DateTime.UtcNow;
                var localtime = now.ToString("o");

                // Act
                var result = Saml20Utils.FromUtcString(localtime);

                // Assert
                Assert.AreEqual(now, result);
            }

            /// <summary>
            /// Verify <see cref="Saml20FormatException"/> is thrown on failure.
            /// </summary>
            [Test]
            [ExpectedException(typeof(Saml20FormatException))]
            public void ThrowsSaml20FormatExceptionOnFailure()
            {
                // Arrange
                var localtime = DateTime.UtcNow.ToString();

                // Act
                Saml20Utils.FromUtcString(localtime);

                // Assert
                Assert.Fail("Conversion from non-UTC string must not succeed");
            }
        }

        /// <summary>
        /// <c>ToUtcString</c> method tests.
        /// </summary>
        [TestFixture]
        public class ToUtcStringMethod
        {
            /// <summary>
            /// Verify can convert UTC formatted string.
            /// </summary>
            [Test]
            public void CanConvertToString()
            {
                // Arrange
                var now = DateTime.UtcNow;
                var localtime = now.ToString("o");

                // Act
                var result = Saml20Utils.ToUtcString(now);

                // Assert
                Assert.AreEqual(localtime, result);
            }
        }
    }
}
