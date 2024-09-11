using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Services.Palette.Readers
{
    internal abstract class AbstractPaletteReader
    {
        protected string _filename;
        public AbstractPaletteReader(string filename)
        {
            _filename = filename;
        }

        public abstract PaletteModel Read();
    }
}
