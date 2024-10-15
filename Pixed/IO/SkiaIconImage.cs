using BluwolfIcons;
using Pixed.Utils;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;

namespace Pixed.IO;
internal class SkiaIconImage : IIconImage
{
    private readonly SKBitmap _bitmap;

    public SkiaIconImage(SKBitmap bitmap)
    {
        _bitmap = bitmap;
    }
    public int Width => _bitmap.Width;

    public int Height => _bitmap.Height;

    public int BitsPerPixel => _bitmap.BytesPerPixel;

    public byte[] GetData()
    {
        MemoryStream stream = new();
        _bitmap.Export(stream);
        IList<byte> data = stream.ToArray();
        stream.Dispose();
        return [.. data];
    }
}