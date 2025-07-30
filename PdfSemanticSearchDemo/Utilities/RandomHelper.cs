using System.Security.Cryptography;

namespace PdfSemanticSearchDemo.Utilities
{
    public static class RandomHelper
    {
        public static ulong GetRandomULong()
        {
            Span<byte> buffer = stackalloc byte[8];
            RandomNumberGenerator.Fill(buffer);
            return BitConverter.ToUInt64(buffer);
        }
    }
}
