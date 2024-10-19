using Pixed.Common.Models;
using System.Collections.Generic;

namespace Pixed.Common.Selection;

internal class ShapeSelection : BaseSelection
{
    public ShapeSelection(List<Pixel> pixels) : base()
    {
        Pixels.AddRange(pixels);
    }
}
