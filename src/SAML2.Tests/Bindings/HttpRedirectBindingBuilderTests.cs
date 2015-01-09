using System;
using NUnit.Framework;
using SAML2.Bindings;

namespace SAML2.Tests.Bindings
{
    /// <summary>
    /// <see cref="HttpRedirectBindingBuilder"/> tests.
    /// </summary>
    [TestFixture]
    public class HttpRedirectBindingBuilderTests
    {
        /// <summary>
        /// Request property tests.
        /// </summary>
        [TestFixture]
        public class RequestProperty
        {
            /// <summary>
            /// Ensure that it is not possible to add a request, when a response has already been added.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void DoesNotAllowResponseAndRequestToBothBeSet()
            {
                // Arrange
                var binding = new HttpRedirectBindingBuilder
                                  {
                                      Response = "Response"
                                  };

                // Act
                binding.Request = "Request";

                // Assert
                Assert.Fail("HttpRedirectBinding did not throw an exception when both Request and Response were set.");
            }
        }

        /// <summary>
        /// Response property tests
        /// </summary>
        [TestFixture]
        public class ResponseProperty
        {
            /// <summary>
            /// Ensure that it is not possible to add a response, when a request has already been added.
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void DoesNotAllowRequestAndResponseToBothBeSet()
            {
                // Arrange
                var binding = new HttpRedirectBindingBuilder
                                  {
                                      Request = "Request"
                                  };

                // Act
                binding.Response = "Response";

                // Assert
                Assert.Fail("HttpRedirectBinding did not throw an exception when both Request and Response were set.");
            }
        }

        /// <summary>
        /// ToQuery method tests.
        /// </summary>
        [TestFixture]
        public class ToQueryMethod
        {
            /// <summary>
            /// Tests that when using the builder to create a response, the relay state is not encoded.
            /// </summary>
            [Test]
            public void DoesNotEncodeRelayStateForResponse()
            {
                // Arrange
                var relaystate = string.Empty.PadRight(10, 'A');
                var bindingBuilder = new HttpRedirectBindingBuilder
                                         {
                                             RelayState = relaystate,
                                             Response = "A random response... !!!! .... "
                                         };

                // Act
                var query = bindingBuilder.ToQuery();

                // Assert
                Assert.That(query.Contains(relaystate));
            }

            /// <summary>
            /// Tests that when using the builder to create a request, the relay state is encoded.
            /// </summary>
            [Test]
            public void EncodesRelayStateForRequests()
            {
                // Arrange
                var relaystate = string.Empty.PadRight(10, 'A');
                var bindingBuilder = new HttpRedirectBindingBuilder
                                         {
                                             Request = "A random request... !!!! .... ",
                                             RelayState = relaystate
                                         };

                // Act
                var query = bindingBuilder.ToQuery();

                // Assert
                Assert.That(!query.Contains(relaystate));
            }
        }
    }
}
