using Pixed.Application.IO;
using System.IO;

namespace Pixed.Android;
internal class AndroidStream(Stream stream) : StreamBase(stream)
{
    private readonly MemoryStream _internalStream = new();
    public override bool CanRead => _internalStream.CanRead;

    public override bool CanSeek => _internalStream.CanSeek;

    public override bool CanWrite => _internalStream.CanWrite;

    public override long Length => _internalStream.Length;

    public override long Position { get => _internalStream.Position; set => _internalStream.Position = value; }

    public override void Flush()
    {
        _internalStream.Flush();
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _internalStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _internalStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _internalStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _internalStream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        var buffer = _internalStream.ToArray();
        _stream.Write(buffer, 0, buffer.Length);
        _internalStream.Dispose();
    }
}
