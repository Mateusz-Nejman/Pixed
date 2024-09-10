using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Services.Palette.Writers
{
    internal abstract class AbstractPaletteWriter
    {
        public abstract void Write(PaletteModel model, string filename);
    }
}
