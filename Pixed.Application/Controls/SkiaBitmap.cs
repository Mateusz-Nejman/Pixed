using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Pixed.Core.Utils;
using SkiaSharp;
using System;

namespace Pixed.Application.Controls;
internal class SkiaBitmap : IImage, IDisposable, ICustomDrawOperation
{
    private readonly SKBitmap? _source;

    public Rect Bounds { get; set; }

    public SkiaBitmap(SKBitmap? source)
    {
        _source = source;
        if (source?.Info.Size is { } size)
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
        if (!SkiaUtils.IsNull(_source) && _source != null && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is { } leaseFeature)
        {
            var lease = leaseFeature.Lease();
            using (lease)
            {
                lease.SkCanvas.DrawBitmapLock(_source, Bounds);
            }
        }
    }
}