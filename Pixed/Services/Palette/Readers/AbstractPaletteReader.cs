using Pixed.Models;

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
