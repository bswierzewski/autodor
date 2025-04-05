using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class FirmaExtensions
    {
        public static string HmacSHA1(this string input, string key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(StringToByteArray(key));
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
