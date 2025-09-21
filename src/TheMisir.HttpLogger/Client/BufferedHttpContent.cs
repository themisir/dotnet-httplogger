using System.Net;
using System.Text;

namespace TheMisir.HttpLogger.Client;

internal sealed class BufferedHttpContent : HttpContent
{
    private byte[]? _bytes;

    private byte[] Bytes => _bytes ?? throw new Exception("BufferedHttpContent has not been initialized");

    public async Task<string> Read(HttpContent source)
    {
        // Copy headers
        foreach (var kvp in source.Headers)
        {
            Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
        }

        // Read body
        _bytes = await source.ReadAsByteArrayAsync().ConfigureAwait(false);

        // Return UTF8 decoded body
        return Encoding.UTF8.GetString(_bytes);
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        return stream.WriteAsync(Bytes).AsTask();
    }

    protected override bool TryComputeLength(out long length)
    {
        length = Bytes.Length;
        return true;
    }
}
