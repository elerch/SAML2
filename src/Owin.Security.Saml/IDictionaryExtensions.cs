using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Security.Saml
{
    public static class IDictionaryExtensions
    {
        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            var nvc = new NameValueCollection(value.Count);
            foreach (var item in value)
                nvc.Add(item.Key.ToString(), item.Value.ToString());
            return nvc;
        }

        public static string ToDelimitedString<TKey, TValue>(this IDictionary<TKey, TValue> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return string.Join("&", value.Select(kvp => string.Format("{0}={1}", kvp.Key, Uri.EscapeDataString(kvp.Value.ToString()))));
        }

        public static IEnumerable<KeyValuePair<string,string>> FromDelimitedString(this string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return value.Split('&').Select(kvp => {
                var split = kvp.Split('=');
                return new KeyValuePair<string, string>(split[0], Uri.UnescapeDataString(split[1]));
            });
        }
    }
}
