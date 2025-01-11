using Pixed.Application.Utils;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using Svg.Skia;
using System;
using System.IO;
using System.Linq;

namespace Pixed.Application.IO;
internal class SvgProjectSerializer : IPixedProjectSerializer
{
    public bool CanSerialize => false;

    public bool CanDeserialize => true;
    public int Width { get; set; } = -1;
    public int Height { get; set; } = -1;

    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        SKSvg svg = SKSvg.CreateFromStream(stream);

        if (Width == -1 || Height == -1)
        {
            Width = (int)svg.Picture.CullRect.Width;
            Height = (int)svg.Picture.CullRect.Height;
        }

        var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var bitmap = new SKBitmap(info);
        using var canvas = new SKCanvas(bitmap);
        canvas.DrawColor(UniColor.Transparent);
        float scaleX = Width.ToFloat() / svg.Picture.CullRect.Width;
        float scaleY = Height.ToFloat() / svg.Picture.CullRect.Height;
        canvas.Save();
        canvas.Scale(scaleX, scaleY);
        canvas.DrawPicture(svg.Picture);
        canvas.Restore();
        var data = bitmap.ToArray();
        canvas.Dispose();
        bitmap.Dispose();

        return PixedModel.FromFrames([Frame.FromLayers([Layer.FromColors(data, Width, Height, "Layer 0")])], applicationData.GenerateName(), applicationData);
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        throw new NotImplementedException();
    }
}