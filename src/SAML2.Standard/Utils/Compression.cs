using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SAML2.Utils
{
    public class Compression
    {
        /// <summary>
        /// Take a Base64-encoded string, decompress the result using the DEFLATE algorithm and return the resulting
        /// string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The decompressed value.</returns>
        public static string DeflateDecompress(string value)
        {
            var encoded = Convert.FromBase64String(value);
            var memoryStream = new MemoryStream(encoded);

            var result = new StringBuilder();
            using (var stream = new DeflateStream(memoryStream, CompressionMode.Decompress)) {
                var testStream = new StreamReader(new BufferedStream(stream), Encoding.UTF8);

                // It seems we need to "peek" on the StreamReader to get it started. If we don't do this, the first call to 
                // ReadToEnd() will return string.empty.
                testStream.Peek();
                result.Append(testStream.ReadToEnd());

                stream.Close();
            }

            return result.ToString();
        }

        /// <summary>
        /// Uses DEFLATE compression to compress the input value. Returns the result as a Base64 encoded string.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>The compressed string.</returns>
        public static string DeflateEncode(string val)
        {
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false))) {
                writer.Write(val);
                writer.Close();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
            }
        }
    }
}
