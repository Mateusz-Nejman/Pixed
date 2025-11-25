using Pixed.Application.IO;

namespace Pixed.Android;
internal class AndroidStreamWrite(Stream stream) : StreamBase(stream)
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
        Stream.Flush();
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
        Java.IO.BufferedOutputStream bos = new(Stream);
        bos.Write(buffer, 0, buffer.Length);
        bos.Close();
        _internalStream.Dispose();
    }
}

internal class AndroidStreamRead(Stream stream) : StreamBase(stream)
{
    public override bool CanRead => Stream.CanRead;

    public override bool CanSeek => Stream.CanSeek;

    public override bool CanWrite => Stream.CanWrite;

    public override long Length => Stream.Length;

    public override long Position { get => Stream.Position; set => Stream.Position = value; }

    public override void Flush()
    {
        Stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return Stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        Stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
        Stream.Dispose();
    }
}

