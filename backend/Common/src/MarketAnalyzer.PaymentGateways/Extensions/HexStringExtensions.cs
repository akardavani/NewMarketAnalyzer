using System.Linq;

namespace System
{
    internal static class HexStringExtensions
    {
        public static byte[] HexStringToByteArray(this string hexString)
            => Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(value: hexString.Substring(startIndex: x, length: 2), fromBase: 16))
                .ToArray();

        public static string ByteArrayToHexString(this byte[] bytes)
            => bytes.Select(t => t.ToString(format: "X2")).Aggregate((a, b) => $"{a}{b}");
    }
}
