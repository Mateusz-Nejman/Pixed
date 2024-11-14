using Pixed.Core.Models;

namespace Pixed.Core.Selection;

public class ShapeSelection : BaseSelection
{
    public ShapeSelection(List<Pixel> pixels) : base()
    {
        Pixels.AddRange(pixels);
    }
}
