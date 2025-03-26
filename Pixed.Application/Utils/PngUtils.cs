using Pixed.BigGustave;
using Pixed.Core.Utils;
using SkiaSharp;
using System.IO;

namespace Pixed.Application.Utils;
internal static class PngUtils
{
    public static void Export(this SKBitmap value, Stream stream)
    {
        SKBitmap convertedBitmap = new(value.Width, value.Height, true);
        byte[] colors;

        if (value.CopyTo(convertedBitmap, SKColorType.Bgra8888))
        {
            colors = convertedBitmap.ToByteArray();
        }
        else
        {
            colors = value.ToByteArray();
        }

        var builder = PngBuilder.FromBgra32Pixels(colors, value.Width, value.Height);
        builder.Save(stream);
        convertedBitmap.Dispose();
    }
}