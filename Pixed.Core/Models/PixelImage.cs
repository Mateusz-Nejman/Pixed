using SkiaSharp;

namespace Pixed.Core.Models;
public abstract class PixelImage : PropertyChangedBase
{
    protected List<Pixel> _modifiedPixels = [];
    protected List<Pixel> ModifiedPixels => _modifiedPixels;

    public virtual void SetModifiedPixels(List<Pixel> modifiedPixels)
    {
        _modifiedPixels.Clear();
        _modifiedPixels.AddRange(modifiedPixels);
    }

    public abstract SKBitmap? Render();
}