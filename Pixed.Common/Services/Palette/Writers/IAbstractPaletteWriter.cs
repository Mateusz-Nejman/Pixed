using Pixed.Common.Models;

namespace Pixed.Common.Services.Palette.Writers;

internal interface IAbstractPaletteWriter
{
    public abstract void Write(PaletteModel model, string filename);
}
