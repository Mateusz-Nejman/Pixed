using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pixed.Application.IO;
internal class IconProjectSerializer : IPixedProjectSerializer
{
    public bool CanSerialize => true;
    public bool CanDeserialize => true;
    public List<Point> IconFormats { get; set; } = [];

    public string FormatExtension => ".ico";

    public string FormatName => "Icon file";

    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        IList<SkiaIconImage> images = [];
        if (model.Frames.Count == 1)
        {
            var bitmap = model.CurrentFrame.Render();

            if (IconFormats.Count == 0)
            {
                images.Add(new SkiaIconImage(bitmap));
            }
            else
            {
                var formats = IconFormats.OrderByDescending(f => f.X + f.Y);

                foreach (var format in formats)
                {
                    if (format.X == bitmap.Width && format.Y == bitmap.Height)
                    {
                        images.Add(new SkiaIconImage(bitmap));
                    }
                    else
                    {
                        var scaledBitmap = new SKBitmap(format.X, format.Y);
                        var canvas = new SKCanvas(scaledBitmap);
                        canvas.Clear(SKColors.Transparent);
                        canvas.DrawBitmapLock(bitmap, SKRect.Create(scaledBitmap.Width, scaledBitmap.Height));
                        canvas.Dispose();
                        images.Add(new SkiaIconImage(scaledBitmap));
                    }
                }
            }
        }
        else
        {
            foreach (var frame in model.Frames)
            {
                images.Add(new SkiaIconImage(frame.Render()));
            }
        }

        Save(stream, images);

        if (close)
        {
            stream.Dispose();
        }
    }

    private static void Save(Stream stream, IList<SkiaIconImage> images)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanWrite)
            throw new ArgumentException("Stream must support writing.", nameof(stream));

        using var writer = new BinaryWriter(stream);
        // Reserved, always 0.
        writer.Write((ushort)0);
        // 1 for ICO, 2 for CUR
        writer.Write((ushort)1);
        writer.Write((ushort)images.Count);

        var pendingImages = new Queue<byte[]>();
        var offset = 6 + 16 * images.Count; // Header: 6; Each Image: 16

        foreach (var image in images)
        {
            writer.Write((byte)image.Width);
            writer.Write((byte)image.Height);

            // Number of colors in the palette. Since we always save the image ourselves (with no palette), hardcode this to 0 (No palette).
            writer.Write((byte)0);
            // Reserved, always 0.
            writer.Write((byte)0);
            // Color planes. Since we save the images ourselves, this is 1.
            writer.Write((ushort)1);
            writer.Write((ushort)image.BitsPerPixel);

            var data = image.GetData();

            writer.Write((uint)data.Length);
            writer.Write((uint)offset);

            offset += data.Length;
            pendingImages.Enqueue(data);
        }

        while (pendingImages.Count > 0)
            writer.Write(pendingImages.Dequeue());
    }
}
