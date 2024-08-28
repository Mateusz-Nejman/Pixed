using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Selection
{
    internal class ShapeSelection : BaseSelection
    {
        public ShapeSelection(List<Pixel> pixels):base()
        {
            Pixels.AddRange(pixels);
        }
    }
}
