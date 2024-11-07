namespace Pixed.BigGustave;

internal class PngStreamWriteHelper(Stream inner) : Stream
{
    private readonly Stream inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly List<byte> written = [];

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    public override void Flush() => inner.Flush();

    public void WriteChunkHeader(byte[] header)
    {
        written.Clear();
        Write(header, 0, header.Length);
    }

    public void WriteChunkLength(int length)
    {
        StreamHelper.WriteBigEndianInt32(inner, length);
    }

    public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);

    public override void SetLength(long value) => inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        written.AddRange(buffer.Skip(offset).Take(count));
        inner.Write(buffer, offset, count);
    }

    public void WriteCrc()
    {
        var result = (int)Crc32.Calculate(written);
        StreamHelper.WriteBigEndianInt32(inner, result);
    }
}