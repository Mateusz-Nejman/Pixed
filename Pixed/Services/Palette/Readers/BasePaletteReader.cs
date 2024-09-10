using Pixed.Models;
using System.IO;

namespace Pixed.Services.Palette.Readers
{
    internal class BasePaletteReader(string filename) : AbstractPaletteReader(filename)
    {
        public override PaletteModel Read()
        {
            return PaletteModel.FromJson(File.ReadAllText(_filename));
        }
    }
}
