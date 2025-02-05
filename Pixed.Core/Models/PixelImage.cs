using SkiaSharp;

namespace Pixed.Core.Models;
public abstract class PixelImage : PropertyChangedBase
{
    protected List<Pixel> _modifiedPixels = [];
    public string UUID { get; protected set; } = string.Empty;
    public List<Pixel> ModifiedPixels => _modifiedPixels;

    public virtual void SetModifiedPixels(List<Pixel> modifiedPixels)
    {
        _modifiedPixels = modifiedPixels;
        UUID = GenerateUUID();
    }

    public abstract SKBitmap Render();
    public abstract bool NeedRender(string uuid);
    public abstract string GenerateUUID();
}