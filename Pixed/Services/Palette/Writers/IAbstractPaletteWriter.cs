using Pixed.Models;

namespace Pixed.Services.Palette.Writers;

internal interface IAbstractPaletteWriter
{
    public abstract void Write(PaletteModel model, string filename);
}
