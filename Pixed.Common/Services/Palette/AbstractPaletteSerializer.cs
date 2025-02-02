using Pixed.Common.Models;
using System.IO;
namespace Pixed.Common.Services.Palette;
public abstract class AbstractPaletteSerializer
{
    public abstract PaletteModel Deserialize(Stream stream, string filename);
    public abstract void Serialize(Stream stream, PaletteModel model);

    public static AbstractPaletteSerializer GetFromExtension(string extension)
    {
        if (extension == ".gpl")
        {
            return new GplPaletteSerializer();
        }
        else if (extension == ".png")
        {
            return new PngPaletteSerializer();
        }

        return new BasePaletteSerializer();
    }
}
