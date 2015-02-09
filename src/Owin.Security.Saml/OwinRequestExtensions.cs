using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Security.Saml
{
    // Source: https://gist.githubusercontent.com/randyburden/42bc688780564ed01107/raw/429b525c72e5465af09d1a0c58913d31c80c7c33/OwinRequestExtensions.cs
    /// <summary>
    /// Owin Request extensions.
    /// </summary>
    public static class OwinRequestExtensions
    {
        /// <summary>
        /// Gets the combined request parameters from the form body, query string, and request headers.
        /// </summary>
        /// <param name="request">Owin request.</param>
        /// <returns>Dictionary of combined form body, query string, and request headers.</returns>
        public static Dictionary<string, string> GetRequestParameters(this IOwinRequest request)
        {
            IEnumerable<KeyValuePair<string,string>> allParams = request.GetBodyParameters();

            var queryParameters = request.GetQueryParameters();

            var headerParameters = request.GetHeaderParameters();

            allParams = allParams.Concat(queryParameters.Except(allParams));

            allParams = allParams.Concat(headerParameters.Except(allParams));

            return allParams.ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// Gets the query string request parameters.
        /// </summary>
        /// <param name="request">Owin Request.</param>
        /// <returns>Dictionary of query string parameters.</returns>
        public static Dictionary<string, string> GetQueryParameters(this IOwinRequest request)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var pair in request.Query) {
                var value = GetJoinedValue(pair.Value);

                dictionary.Add(pair.Key, value);
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the form body request parameters.
        /// </summary>
        /// <param name="request">Owin Request.</param>
        /// <returns>Dictionary of form body parameters.</returns>
        public static Dictionary<string, string> GetBodyParameters(this IOwinRequest request)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            var formCollectionTask = request.ReadFormAsync();

            formCollectionTask.Wait();

            foreach (var pair in formCollectionTask.Result) {
                var value = GetJoinedValue(pair.Value);

                dictionary.Add(pair.Key, value);
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the header request parameters.
        /// </summary>
        /// <param name="request">Owin Request.</param>
        /// <returns>Dictionary of header parameters.</returns>
        public static Dictionary<string, string> GetHeaderParameters(this IOwinRequest request)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var pair in request.Headers) {
                var value = GetJoinedValue(pair.Value);

                dictionary.Add(pair.Key, value);
            }

            return dictionary;
        }

        private static string GetJoinedValue(string[] value)
        {
            if (value != null)
                return string.Join(",", value);

            return null;
        }
    }
}
