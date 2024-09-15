using Pixed.Models;

namespace Pixed.Services.Palette.Readers;

internal abstract class AbstractPaletteReader(string filename)
{
    protected string _filename = filename;

    public abstract PaletteModel Read();
}
