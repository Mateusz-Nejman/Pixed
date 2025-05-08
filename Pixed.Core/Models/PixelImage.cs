using SkiaSharp;

namespace Pixed.Core.Models;
public abstract class PixelImage : PropertyChangedBase
{
    protected List<Pixel> _modifiedPixels = [];
    protected List<Pixel> ModifiedPixels => _modifiedPixels;

    public virtual string RenderId { get; set; } = string.Empty;

    public abstract int Width { get; }
    public abstract int Height { get; }

    public virtual void SetModifiedPixels(List<Pixel> modifiedPixels)
    {
        _modifiedPixels.Clear();
        _modifiedPixels.AddRange(modifiedPixels);
    }

    public virtual void ResetID()
    {
        RenderId = Guid.NewGuid().ToString();
    }

    public abstract SKBitmap? Render();
}