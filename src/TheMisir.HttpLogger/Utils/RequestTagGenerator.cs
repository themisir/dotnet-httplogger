using System.Security.Cryptography;
using TheMisir.HttpLogger.Models;

namespace TheMisir.HttpLogger.Utils;

internal sealed class RequestTagGenerator(string scopeName)
{
    public RequestTag Next()
    {
        Span<byte> s = stackalloc byte[sizeof(uint)];
        RandomNumberGenerator.Fill(s);
        var requestId = BitConverter.ToUInt32(s);
        return new RequestTag(scopeName, requestId);
    }
}
