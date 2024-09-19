using BigGustave;
using Pixed.Models;
using System.IO;

namespace Pixed.IO
{
    internal class PngProjectSerializer : IPixedProjectSerializer
    {
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
            return PixedModel.FromFrames([frame]);
        }

        public void Serialize(Stream stream, PixedModel model, bool close)
        {
            model.Clone();

            var builder = PngBuilder.Create(model.Width * model.Frames.Count, model.Height, true);

            for(int a = 0; a < model.Frames.Count; a++)
            {
                //TODO optimize
                Frame frame = model.Frames[a];
                Layer layer = Layer.MergeAll([.. frame.Layers]);
                var pixels = layer.GetPixels();
                for(int x = 0; x < model.Width; x++)
                {
                    for(int y = 0; y < model.Height; y++)
                    {
                        UniColor color = pixels[y * model.Width + x];
                        var pixel = new BigGustave.Pixel(color.R, color.G, color.B, color.A, false);
                        builder.SetPixel(pixel, x + (model.Width * a), y);
                    }
                }
            }

            builder.Save(stream);
        }
    }
}
