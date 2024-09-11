using Pixed.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
