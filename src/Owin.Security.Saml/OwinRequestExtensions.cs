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

            var formCollectionTask = ReadFormAsync(request);

            formCollectionTask.Wait();

            foreach (var pair in formCollectionTask.Result) {
                var value = GetJoinedValue(pair.Value);

                dictionary.Add(pair.Key, value);
            }

            return dictionary;
        }

        public static Task<IFormCollection> ReadFormAsync(this IOwinRequest value)
        {
            var form = value.Get<IFormCollection>("Microsoft.Owin.Form#collection");
            if (form == null) {
                value.Body.Seek(0, System.IO.SeekOrigin.Begin); // needed so ReadToEndAsync() doesn't hang
                string text;
                // Don't close, it prevents re-winding.
                using (var reader = new System.IO.StreamReader(value.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4 * 1024, leaveOpen: true)) {
                    // *not* async - we're seeing hangs.  See another with this issue: https://github.com/damianh/LibOwin/issues/12
                    text = reader.ReadToEnd();
                }
                value.Body.Seek(0, System.IO.SeekOrigin.Begin); // subsequent calls will return nothing if this isn't re-wound
                form = GetForm(text);
                value.Set("Microsoft.Owin.Form#collection", form);
            }
            return Task.FromResult(form);
        }

        #region Lifted from Katana project internal class. Microsoft.Owin.Infrastructure.OwinHelpers
        internal static IFormCollection GetForm(string text)
        {
            IDictionary<string, string[]> form = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            var accumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            ParseDelimited(text, new[] { '&' }, AppendItemCallback, accumulator);
            foreach (var kv in accumulator) {
                form.Add(kv.Key, kv.Value.ToArray());
            }
            return new FormCollection(form);
        }

        private static readonly Action<string, string, object> AppendItemCallback = (name, value, state) => {
            var dictionary = (IDictionary<string, List<String>>)state;

            List<string> existing;
            if (!dictionary.TryGetValue(name, out existing)) {
                dictionary.Add(name, new List<string>(1) { value });
            } else {
                existing.Add(value);
            }
        };

        internal static void ParseDelimited(string text, char[] delimiters, Action<string, string, object> callback, object state)
        {
            int textLength = text.Length;
            int equalIndex = text.IndexOf('=');
            if (equalIndex == -1) {
                equalIndex = textLength;
            }
            int scanIndex = 0;
            while (scanIndex < textLength) {
                int delimiterIndex = text.IndexOfAny(delimiters, scanIndex);
                if (delimiterIndex == -1) {
                    delimiterIndex = textLength;
                }
                if (equalIndex < delimiterIndex) {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex])) {
                        ++scanIndex;
                    }
                    string name = text.Substring(scanIndex, equalIndex - scanIndex);
                    string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
                    callback(
                        Uri.UnescapeDataString(name.Replace('+', ' ')),
                        Uri.UnescapeDataString(value.Replace('+', ' ')),
                        state);
                    equalIndex = text.IndexOf('=', delimiterIndex);
                    if (equalIndex == -1) {
                        equalIndex = textLength;
                    }
                }
                scanIndex = delimiterIndex + 1;
            }
        }
        #endregion
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
