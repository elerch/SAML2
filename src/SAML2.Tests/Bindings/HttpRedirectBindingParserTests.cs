using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using SAML2.Bindings;

namespace SAML2.Tests.Bindings
{
    /// <summary>
    /// <see cref="HttpRedirectBindingParser"/> tests.
    /// </summary>
    [TestFixture]
    public class HttpRedirectBindingParserTests
    {
        /// <summary>
        /// Performs a simple split of an Url query, and stores the result in a NameValueCollection.
        /// This method may fail horribly if the query string is not correctly URL-encoded.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="NameValueCollection"/> that results from parsing the query.</returns>
        private static NameValueCollection QueryToNameValueCollection(string request)
        {
            if (request[0] == '?')
            {
                request = request.Substring(1);
            }

            var result = new NameValueCollection();
            foreach (var s in request.Split('&'))
            {
                var keyvalue = s.Split('=');
                result.Add(keyvalue[0], keyvalue[1]);
            }

            return result;
        }

        /// <summary>
        /// Constructor method tests.
        /// </summary>
        [TestFixture]
        public class ConstructorMethod
        {
            /// <summary>
            /// Verify that basic encoding and decoding of a Request works.
            /// Verify that the parser correctly detects a Request parameter.
            /// </summary>
            [Test]
            public void ParserCanEncodeAndDecodeRequest()
            {
                // Arrange
                var request = string.Empty.PadLeft(350, 'A') + "ÆØÅæøå";
                var bindingBuilder = new SAML2.Bindings.HttpRedirectBindingBuilder { Request = request };

                var query = bindingBuilder.ToQuery();
                var coll = QueryToNameValueCollection(query);
                var url = new Uri("http://localhost/?" + query);

                // Act
                var bindingParser = new HttpRedirectBindingParser(url);

                // Assert
                Assert.That(coll.Count == 1);
                Assert.That(bindingParser.IsRequest);
                Assert.That(!bindingParser.IsResponse);
                Assert.That(!bindingParser.IsSigned);
                Assert.AreEqual(request, bindingParser.Message);
            }

            /// <summary>
            /// Verify that basic encoding and decoding of a RelayState works.
            /// </summary>
            [Test]
            public void ParserCanEncodeAndDecodeRequestWithRelayState()
            {
                // Arrange
                var request = string.Empty.PadRight(140, 'l');
                var relaystate = "A relaystate test. @@@!!!&&&///";

                var bindingBuilder = new SAML2.Bindings.HttpRedirectBindingBuilder
                                         {
                                             Request = request,
                                             RelayState = relaystate
                                         };

                var query = bindingBuilder.ToQuery();
                var coll = QueryToNameValueCollection(query);
                var url = new Uri("http://localhost/?" + query);

                // Act
                var bindingParser = new HttpRedirectBindingParser(url);

                // Assert
                Assert.AreEqual(2, coll.Count);
                Assert.IsTrue(bindingParser.IsRequest);
                Assert.IsFalse(bindingParser.IsResponse);
                Assert.IsFalse(bindingParser.IsSigned);
                Assert.IsNotNull(bindingParser.RelayState);
                Assert.AreEqual(relaystate, bindingParser.RelayStateDecoded);
                Assert.AreEqual(request, bindingParser.Message);
            }

            /// <summary>
            /// Uses a DSA key to sign and verify the Authentication request.
            /// </summary>
            [Test]
            public void ParserCanSignAuthnRequestWithDsaKey()
            {
                // Arrange
                var key = new DSACryptoServiceProvider();
                var evilKey = new DSACryptoServiceProvider();

                var binding = new SAML2.Bindings.HttpRedirectBindingBuilder
                {
                    Request = string.Empty.PadLeft(500, 'a'),
                    SigningKey = key
                };

                var url = new Uri("http://localhost/?" + binding.ToQuery());

                // Act
                var parser = new HttpRedirectBindingParser(url);

                // Assert
                Assert.That(parser.IsSigned);
                Assert.That(parser.IsRequest);
                Assert.That(parser.CheckSignature(key));
                Assert.IsFalse(parser.CheckSignature(evilKey));
            }

            /// <summary>
            /// Uses a RSA key to sign and verify the Authentication request.
            /// </summary>
            [Test]
            public void ParserCanSignAuthnRequestWithRsaKey()
            {
                // Arrange
                var key = new RSACryptoServiceProvider();
                var evilKey = new RSACryptoServiceProvider();

                var binding = new SAML2.Bindings.HttpRedirectBindingBuilder
                {
                    Request = string.Empty.PadLeft(500, 'a'),
                    SigningKey = key
                };

                var url = new Uri("http://localhost/?" + binding.ToQuery());

                // Act
                var parser = new HttpRedirectBindingParser(url);

                // Assert
                Assert.That(parser.IsSigned);
                Assert.That(parser.IsRequest);
                Assert.That(!parser.IsResponse);
                Assert.That(parser.CheckSignature(key));
                Assert.IsFalse(parser.CheckSignature(evilKey));
            }
        }

        /// <summary>
        /// CheckSignature method tests.
        /// </summary>
        [TestFixture]
        public class CheckSignatureMethod
        {
            /// <summary>
            /// Tests an actual Ping Federate signed response response can be parsed.
            /// </summary>
            [Test]
            public void ParserCanParsePingFederateSignedResponse()
            {
                // Arrange
                var url = new Uri("http://haiku.safewhere.local/Saml20TestWeb/SSOLogout.saml2.aspx?SAMLResponse=fZFRa8IwEMe%2FSsm7bZq2qMEWZN1DwSEY0eGLpGmqZTUpuYTpt19bGVMYPob7%2FX%2BXu1sAv7QdXemTdnYjodMKpJdLsI3ittEqRWdrOxoEZ958OR94Lb%2FP0ki%2F1YK3AevjBG97fi%2FLgLH13eQPWuJz6K7IK9SveKtT1FQ1qYSoSSRIVMVhPJ3hpMQRj6IwKUVcJn0CwMlCgeXKpohgPJvgaILjbUhoQmg49cl8dui5PEWH%2BoaB6P2arAtlq%2FqWF%2FNTXn8y3yBvJw2MQxAfI%2B96aRXQceIUOaOo5tAAVfwigVpB2fJjRXuSdkZbLXSLssVA0%2FE%2F5iH%2FOs4BpBmWh7JlvnrfHIcKwcciXwQPvru8o8xy6%2BD59aYr6e146%2BTrVjDSlDkhJAAKsnuHP2nw34GzHw%3D%3D&SigAlg=http%3A%2F%2Fwww.w3.org%2F2000%2F09%2Fxmldsig%23rsa-sha1&Signature=UoYGLeSCYOSvjIaBpTcgtq2O0Nbz%2BVk%2BaaLESje8%2FZKxGNmWrFXJjSPrA403J23NeQzbxxVgOwSP8idIM95BhlVwxpiG%2B7%2FhJyNNrjGPohmD3cQpBWoWqZ8IEudDc%2FwDCshPb6wTdr6%2FOdKXQ2uwSK5NA2LYI8AAN5sq9kPtVvk%3D");
                var parser = new HttpRedirectBindingParser(url);
                var cert = new X509Certificate2(@"Certificates\pingcertificate.crt");

                // Act
                var result = parser.CheckSignature(cert.PublicKey.Key);

                // Assert
                Assert.That(result);
            }

            /// <summary>
            /// Tests an actual Ping Federate signed request response can be parsed.
            /// </summary>
            [Test]
            public void ParserCanParsePingFederateSignedRequest()
            {
                // Arrange
                var url = new Uri("https://adler.safewhere.local:9031/idp/SSO.saml2?SAMLRequest=7b0HYBxJliUmL23Ke39K9UrX4HShCIBgEyTYkEAQ7MGIzeaS7B1pRyMpqyqBymVWZV1mFkDM7Z28995777333nvvvfe6O51OJ%2fff%2fz9cZmQBbPbOStrJniGAqsgfP358Hz8iHv8e7xZlepnXTVEtP%2ftod7zz0e9x9Bsnj3%2fR7qPjdTtfvsp%2f0Tpv2vTs6WcfFbP7ew8fPnhw%2f97%2bZC%2fbvzebPty59%2bn92b37kwc755O9g92P0p80kPYIUnrWNOv8bNm02bKlj3Z2DrZ37m3v7L%2fZ3X90b%2ffRvU%2fHDz69v%2fPw3t5PfZQSHsvm0S%2fa%2feyjdb18VGVN0TxaZou8edROH70%2b%2fuL5IwL5aFVXbTWtyo8IyTR9zB3U8u7mF7OmyeuWUPvoCM2%2bnRVv14%2fvyvsC66Razgq0aN4THt6m94%2fXsyJfTvNXRK%2b6mOI7%2fcr70u%2fcfqYA7AddCI%2fvOtwwOXc7s3P0%2fwA%3d&SigAlg=http%3a%2f%2fwww.w3.org%2f2000%2f09%2fxmldsig%23rsa-sha1&Signature=UsZV%2bFga0YfCQaozLomKfV8jyNt85GMIYLFoBA9jrwFfabL%2bpAWVmlhwHyAMv50uxJWFc57v2ySj5Pc6e1t0NyyaguRL8VOKqB4P3svXV5U4iU0Gq4Rp1SJu0bj538%2f01X8IINmcAJMLdrx1cqCoRmofEcPPoQODWhQoq%2brjZdE%3d");
                var parser = new SAML2.Bindings.HttpRedirectBindingParser(url);
                var cert = new X509Certificate2(@"Certificates\SafewhereTest_SFS.pfx", "test1234");

                // Act
                var result = parser.CheckSignature(cert.PublicKey.Key);

                // Assert
                Assert.That(result);
            }

            /// <summary>
            /// Verify parser throws exception on trying to verify signature of unsigned request.
            /// </summary>
            [Test]
            [ExpectedException(typeof(InvalidOperationException))]
            public void ParserThrowsExceptionWhenTryingToVerifySignatureOfUnsignedRequest()
            {
                // Arrange
                var request = string.Empty.PadLeft(350, 'A') + "ÆØÅæøå";
                var bindingBuilder = new SAML2.Bindings.HttpRedirectBindingBuilder { Request = request };

                var query = bindingBuilder.ToQuery();
                var url = new Uri("http://localhost/?" + query);
                var bindingParser = new HttpRedirectBindingParser(url);

                // Act
                bindingParser.CheckSignature(new RSACryptoServiceProvider());

                // Assert
                Assert.Fail("Trying to verify signature of an unsigned request should have thrown an exception.");
            }
        }
    }
}
