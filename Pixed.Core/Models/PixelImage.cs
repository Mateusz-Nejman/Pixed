using SkiaSharp;

namespace Pixed.Core.Models;
public abstract class PixelImage : PropertyChangedBase
{
    protected List<Pixel> _modifiedPixels = [];
    protected SKBitmap? _render = null;

    public virtual bool NeedRender { get; protected set; } = true;
    public List<Pixel> ModifiedPixels => _modifiedPixels;

    public virtual void SetModifiedPixels(List<Pixel> modifiedPixels)
    {
        _modifiedPixels = modifiedPixels;
        NeedRender = true;
    }

    public virtual SKBitmap Render()
    {
        return _render;
    }
}