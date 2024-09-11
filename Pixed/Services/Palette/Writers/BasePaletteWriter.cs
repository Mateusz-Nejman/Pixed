using Pixed.Models;
using System.IO;

namespace Pixed.Services.Palette.Writers
{
    internal class BasePaletteWriter : AbstractPaletteWriter
    {
        public override void Write(PaletteModel model, string filename)
        {
            File.WriteAllText(filename, model.ToJson());
        }
    }
}
