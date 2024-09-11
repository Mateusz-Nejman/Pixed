using Pixed.Models;
using System.IO;

namespace Pixed.Services.Palette.Readers
{
    internal class BasePaletteReader(string filename) : AbstractPaletteReader(filename)
    {
        public override PaletteModel Read()
        {
            PaletteModel model = PaletteModel.FromJson(File.ReadAllText(_filename));
            model.Path = _filename;
            return model;
        }
    }
}
