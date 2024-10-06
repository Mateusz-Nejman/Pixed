using Pixed.Models;
using Pixed.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Pixed.IO;
internal class PngProjectSerializer : IPixedProjectSerializer
{
    public int ColumnsCount { get; set; } = 1;
    public PixedModel Deserialize(Stream stream, ApplicationData applicationData)
    {
        Bitmap bitmap = (Bitmap)Image.FromStream(stream);
        var colors = bitmap.ToPixelColors();

        Layer layer = Layer.FromColors(colors, bitmap.Width, bitmap.Height, "Layer 0");
        Frame frame = Frame.FromLayers([layer]);
        bitmap.Dispose();
        return PixedModel.FromFrames([frame], applicationData.GenerateName(), applicationData);
    }

    public void Serialize(Stream stream, PixedModel model, bool close)
    {
        model.Clone();

        int rows = (int)Math.Ceiling((double)model.Frames.Count / (double)ColumnsCount);
        int width = model.Width * ColumnsCount;
        int height = model.Height * rows;
        Bitmap outputBitmap = new Bitmap(width, height);
        Graphics graphics = Graphics.FromImage(outputBitmap);

        int frameColumn = 0;
        int frameRow = 0;
        for (int a = 0; a < model.Frames.Count; a++)
        {
            //TODO optimize
            Frame frame = model.Frames[a];
            var frameBitmap = frame.Render();
            int x1 = frameColumn * model.Width;
            int y1 = frameRow * model.Height;
            graphics.DrawImage(frameBitmap, new Point(x1, y1));
            frameColumn++;

            if (frameColumn == ColumnsCount)
            {
                frameColumn = 0;
                frameRow++;
            }
        }

        graphics.Dispose();
        outputBitmap.Save(stream, ImageFormat.Png);
        outputBitmap.Dispose();

        if (close)
        {
            stream.Dispose();
        }
    }
}
