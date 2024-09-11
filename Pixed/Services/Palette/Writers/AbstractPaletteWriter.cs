using Pixed.Models;

namespace Pixed.Services.Palette.Writers
{
    internal abstract class AbstractPaletteWriter
    {
        public abstract void Write(PaletteModel model, string filename);
    }
}
