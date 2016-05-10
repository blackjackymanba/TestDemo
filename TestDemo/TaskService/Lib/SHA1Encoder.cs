using System.Text;

namespace GPMGateway.Common.Lib
{
    public class SHA1Encoder
    {
        private static readonly char[] HEX_DIGITS = { '0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        private static string GetFormattedText(byte[] bytes)
        {
            int len = bytes.Length;
            var buf = new StringBuilder(len * 2);
            for (int j = 0; j < len; j++)
            {
                buf.Append(HEX_DIGITS[(bytes[j] >> 4) & 0x0f]);
                buf.Append(HEX_DIGITS[bytes[j] & 0x0f]);
            }
            return buf.ToString();
        }

        public static string Encode(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var hash = System.Security.Cryptography.SHA1.Create();
            return GetFormattedText(hash.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }
    }
}