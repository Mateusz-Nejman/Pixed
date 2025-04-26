using Pixed.Application.Utils;
using Pixed.BigGustave;
using Pixed.Common.Utils;
using Pixed.Core.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Pixed.Application.IO;
public class PngProjectSerializer : IPixedProjectSerializer
{
    public bool CanSerialize => true;
    public bool CanDeserialize => true;
    public int ColumnsCount { get; set; } = 1;
    public int TileWidth { get; set; } = -1;
    public int TileHeight { get; set; } = -1;

    public string FormatExtension => ".png";

    public string FormatName => "PNG image";

    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        var png = Png.Open(stream);
        return Deserialize(png, applicationData);
    }

    public PixedModel Deserialize(Png png, ApplicationData applicationData)
    {
        var colors = png.GetPixelsUInt();
        ObservableCollection<Frame> frames = [];

        Layer layer = Layer.FromColors(colors, png.Width, png.Height, "Layer 0");
        if (TileWidth == -1 && TileHeight == -1)
        {
            Frame frame = Frame.FromLayers([layer]);
            frames.Add(frame);
        }
        else
        {
            for (int x = 0; x < layer.Width; x += TileWidth)
            {
                for (int y = 0; y < layer.Height; y += TileHeight)
                {
                    Layer subLayer = Layer.FromColors(layer.GetRectangleColors(new Point(x, y), new Point(TileWidth, TileHeight)), TileWidth, TileHeight, "Layer 0");
                    Frame subFrame = Frame.FromLayers([subLayer]);
                    frames.Add(subFrame);
                }
            }
        }

        return PixedModel.FromFrames(frames, applicationData.GenerateName(), applicationData);
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        int rows = (int)Math.Ceiling((double)model.Frames.Count / (double)ColumnsCount);
        int width = model.Width * ColumnsCount;
        int height = model.Height * rows;
        SKBitmap outputBitmap = new(width, height, true);
        SKCanvas canvas = new(outputBitmap);

        int frameColumn = 0;
        int frameRow = 0;
        for (int a = 0; a < model.Frames.Count; a++)
        {
            //TODO optimize
            Frame frame = model.Frames[a];
            var frameBitmap = frame.Render();
            int x1 = frameColumn * model.Width;
            int y1 = frameRow * model.Height;
            canvas.DrawBitmap(frameBitmap, new SKPoint(x1, y1));
            frameColumn++;

            if (frameColumn == ColumnsCount)
            {
                frameColumn = 0;
                frameRow++;
            }
        }

        canvas.Dispose();
        outputBitmap.Export(stream);
        outputBitmap.Dispose();

        if (close)
        {
            stream.Dispose();
        }
    }
}
