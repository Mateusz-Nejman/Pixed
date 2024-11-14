using Pixed.BigGustave;
using Pixed.Core.Utils;
using SkiaSharp;
using System.IO;

namespace Pixed.Application.Utils;
internal static class PngUtils
{
    public static void Export(this SKBitmap value, Stream stream)
    {
        var colors = value.ToByteArray();
        var builder = PngBuilder.FromBgra32Pixels(colors, value.Width, value.Height);
        builder.Save(stream);
    }
}