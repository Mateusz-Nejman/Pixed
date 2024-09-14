using Pixed.Models;
using System.Collections.Generic;

namespace Pixed.Selection;

internal class ShapeSelection : BaseSelection
{
    public ShapeSelection(List<Pixel> pixels) : base()
    {
        Pixels.AddRange(pixels);
    }
}
