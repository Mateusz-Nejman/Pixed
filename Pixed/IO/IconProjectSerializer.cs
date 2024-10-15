using Pixed.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Pixed.IO;
internal class IconProjectSerializer : IPixedProjectSerializer
{
    public bool CanSerialize => true;
    public bool CanDeserialize => true;
    public List<Point> IconFormats { get; set; } = [];
    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        BluwolfIcons.Icon icon = new();

        if (model.Frames.Count == 1)
        {
            var bitmap = model.CurrentFrame.Render();

            if (IconFormats.Count == 0)
            {
                icon.Images.Add(new SkiaIconImage(bitmap));
            }
            else
            {
                var formats = IconFormats.OrderByDescending(f => f.X + f.Y);

                foreach (var format in formats)
                {
                    if (format.X == bitmap.Width && format.Y == bitmap.Height)
                    {
                        icon.Images.Add(new SkiaIconImage(bitmap));
                    }
                    else
                    {
                        var scaledBitmap = new SKBitmap(format.X, format.Y, true);
                        var canvas = new SKCanvas(scaledBitmap);
                        canvas.Clear(SKColors.Transparent);
                        canvas.DrawBitmap(bitmap, new SKRect(0, 0, scaledBitmap.Width, scaledBitmap.Height));
                        canvas.Dispose();
                        icon.Images.Add(new SkiaIconImage(scaledBitmap));
                    }
                }
            }
        }
        else
        {
            foreach (var frame in model.Frames)
            {
                icon.Images.Add(new SkiaIconImage(frame.Render()));
            }
        }

        icon.Save(stream);

        if (close)
        {
            stream.Dispose();
        }
    }
}
