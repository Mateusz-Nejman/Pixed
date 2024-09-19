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

        }
    }
}
