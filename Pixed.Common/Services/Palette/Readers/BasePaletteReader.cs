using Pixed.Models;
using System.IO;

namespace Pixed.Services.Palette.Readers;

internal class BasePaletteReader(string filename) : AbstractPaletteReader(filename)
{
    public override PaletteModel Read()
    {
        FileInfo info = new(_filename);
        PaletteModel model = PaletteModel.FromJson(File.ReadAllText(_filename), info.Name);
        model.Path = _filename;
        return model;
    }
}
