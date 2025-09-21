using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TheMisir.HttpLogger.Server;

internal sealed class InterceptedResponseStream(Stream target) : Stream
{
    private readonly MemoryStream _buffer = new();

    public string GetContents()
    {
        return Encoding.UTF8.GetString(_buffer
            .GetBuffer()
            .AsSpan(0, (int)_buffer.Length));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            target.Dispose();
            _buffer.Dispose();
        }
    }

    public override void Flush()
    {
        target.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return target.FlushAsync(cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ThrowNotSupported();
        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ThrowNotSupported();
        return 0;
    }

    public override void SetLength(long value)
    {
        ThrowNotSupported();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        target.Write(buffer, offset, count);
        _buffer.Write(buffer, offset, count);
    }

    public override async ValueTask WriteAsync(
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        await target.WriteAsync(buffer, cancellationToken);
        await _buffer.WriteAsync(buffer, cancellationToken);
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => target.Length;

    public override long Position
    {
        get => target.Position;
        // ReSharper disable once ValueParameterNotUsed
        set => ThrowNotSupported();
    }

    [DoesNotReturn]
    private static void ThrowNotSupported()
    {
        throw new NotSupportedException("InterceptedResponseStream is a write only stream");
    }
}