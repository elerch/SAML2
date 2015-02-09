using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Security.Saml
{
    public static class IReadableStringCollectionExtensions
    {
        public static NameValueCollection ToNameValueCollection(this IReadableStringCollection value)
        {
            if (value == null) throw new ArgumentNullException("value");
            var nvc = new NameValueCollection();
            foreach (var item in value)
                nvc.Add(item.Key, item.Value.FirstOrDefault());
            return nvc;
        }
    }
}
