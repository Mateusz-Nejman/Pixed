using Avalonia.Media;
using Avalonia;
using SkiaSharp;
using System;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Platform;
using Avalonia.Skia;

namespace Pixed.Application.Controls;
internal class SkiaBitmap : IImage, IDisposable, ICustomDrawOperation
{
    private readonly SKBitmap? _source;

    public Rect Bounds { get; set; }

    public SkiaBitmap(SKBitmap? source)
    {
        _source = source;
        if (source?.Info.Size is SKSizeI size)
        {
            Size = new(size.Width, size.Height);
        }
    }

    public Size Size { get; }

    public void Dispose() => _source?.Dispose();

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        Bounds = destRect;
        context.Custom(this);
    }

    public bool Equals(ICustomDrawOperation? other) => false;

    public bool HitTest(Point p) => Bounds.Contains(p);

    public void Render(ImmediateDrawingContext context)
    {
        if (_source is SKBitmap bitmap && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
        {
            ISkiaSharpApiLease lease = leaseFeature.Lease();
            using (lease)
            {
                lease.SkCanvas.DrawBitmap(bitmap, SKRect.Create((float)Bounds.X, (float)Bounds.Y, (float)Bounds.Width, (float)Bounds.Height));
            }
        }
    }
}