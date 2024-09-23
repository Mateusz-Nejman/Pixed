using BigGustave;
using Pixed.Models;
using System;
using System.IO;

namespace Pixed.IO
{
    internal class PngProjectSerializer : IPixedProjectSerializer
    {
        public int ColumnsCount { get; set; } = 1;
        public PixedModel Deserialize(Stream stream)
        {
            Png image = Png.Open(stream);
            int width = image.Width;
            int height = image.Height;
            int[] pixels = new int[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    UniColor color = new(pixel.A, pixel.R, pixel.G, pixel.B);
                    pixels[y * width + x] = color;
                }
            }

            Layer layer = Layer.FromColors(pixels, width, height, "Layer 0");
            Frame frame = Frame.FromLayers([layer]);
            return PixedModel.FromFrames([frame], Global.NamingService.GenerateName());
        }

        public void Serialize(Stream stream, PixedModel model, bool close)
        {
            model.Clone();

            int rows = (int)Math.Ceiling((double)model.Frames.Count / (double)ColumnsCount);
            int width = model.Width * ColumnsCount;
            int height = model.Height * rows;
            var builder = PngBuilder.Create(width, height, true);

            int frameColumn = 0;
            int frameRow = 0;
            for (int a = 0; a < model.Frames.Count; a++)
            {
                //TODO optimize
                Frame frame = model.Frames[a];
                Layer layer = Layer.MergeAll([.. frame.Layers]);
                var pixels = layer.GetPixels();
                int x1 = frameColumn * model.Width;
                int y1 = frameRow * model.Height;
                for (int x = 0; x < model.Width; x++)
                {
                    for (int y = 0; y < model.Height; y++)
                    {
                        UniColor color = pixels[y * model.Width + x];
                        var pixel = new BigGustave.Pixel(color.R, color.G, color.B, color.A, false);
                        builder.SetPixel(pixel, x1 + x, y1 + y);
                    }
                }

                frameColumn++;

                if (frameColumn == ColumnsCount)
                {
                    frameColumn = 0;
                    frameRow++;
                }
            }

            builder.Save(stream);

            if (close)
            {
                stream.Dispose();
            }
        }
    }
}
