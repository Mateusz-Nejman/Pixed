using Pixed.Common.Models;

namespace Pixed.Common.Services.Palette.Readers;

internal abstract class AbstractPaletteReader(string filename)
{
    protected string _filename = filename;

    public abstract PaletteModel Read();
}
