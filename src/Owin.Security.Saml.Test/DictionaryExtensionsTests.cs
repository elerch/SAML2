using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Owin.Security.Saml.Test
{
    /// <summary>
    /// Unit test for <see cref="IDictionaryExtensions"/>
    /// </summary>
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void EscapesValuesProperly()
        {
            const string key = ".return";
            const string value = "http://localhost:9000/Auth/SignIn?returnUrl=http%3A%2F%2Flocalhost%3A9000%2F%23%2Fhome";
            
            
            // arrange
            var attributes = new Dictionary<string, string>()
            {
                {key, value}
            };

            // act
            var attributesAsString = attributes.ToDelimitedString();
            var newAttributesFromString = attributesAsString.FromDelimitedString().ToDictionary(x => x.Key, x => x.Value);

            // assert
            Assert.AreEqual(attributes[key], newAttributesFromString[key]);
        }
    }
}
