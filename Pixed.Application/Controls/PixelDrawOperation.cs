using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Pixed.Core.Models;
using Pixed.Core.Utils;

namespace Pixed.Application.Controls;

public class PixelDrawOperation : IImage, ICustomDrawOperation
{
    private PixelImage? _source;
    private Size _size;

    public Size Size => _size;

    public Rect Bounds { get; set; }

    public PixelImage? Source => _source;

    public PixelDrawOperation() : this(null)
    {
    }
    public PixelDrawOperation(PixelImage? source)
    {
        UpdateBitmap(source);
    }

    public void UpdateBitmap(PixelImage? source)
    {
        _source = source;
        if (source is PixelImage image)
        {
            var bitmap = image.Render();

            if (bitmap != null)
            {
                _size = new(bitmap.Width, bitmap.Height);
            }
        }
    }

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        Bounds = destRect;
        context.Custom(this);
    }

    public bool Equals(ICustomDrawOperation? other) => false;

    public bool HitTest(Avalonia.Point p) => Bounds.Contains(p);

    public void Render(ImmediateDrawingContext context)
    {
        if (Source != null && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
        {
            ISkiaSharpApiLease lease = leaseFeature.Lease();
            using (lease)
            {
                var bitmap = Source.Render();

                if (!SkiaUtils.IsNull(bitmap))
                {
                    lease.SkCanvas.DrawBitmapLock(bitmap, Bounds);
                }
            }
        }
    }

    public void Dispose()
    {
    }
}