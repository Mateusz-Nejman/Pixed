using Pixed.BigGustave;
using Pixed.Common.Models;
using System.IO;
namespace Pixed.Common.Services.Palette;
internal class PngPaletteSerializer : AbstractPaletteSerializer
{
    public override PaletteModel Deserialize(Stream stream, string filename)
    {
        Png png = Png.Open(stream);
        PaletteModel model = new(filename);
        bool colorLimit = false;

        for (int y = 0; y < png.Height; y++)
        {
            for (int x = 0; x < png.Width; x++)
            {
                var color = png.GetPixel(x, y);

                if (!model.Colors.Contains(color))
                {
                    model.Colors.Add(color);
                }

                if (model.Colors.Count == 256)
                {
                    colorLimit = true;
                    break;
                }
            }

            if (colorLimit)
            {
                break;
            }
        }

        return model;
    }

    public override void Serialize(Stream stream, PaletteModel model)
    {
        PngBuilder builder = PngBuilder.Create(model.Colors.Count, 1, true);

        for (int c = 0; c < model.Colors.Count; c++)
        {
            var color = model.Colors[c];
            builder.SetPixel(color, c, 0);
        }

        builder.Save(stream);
    }
}