using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System;

namespace Pixed.Application.Controls;

internal class PixelImageOperation : ICustomDrawOperation
{
    private bool _disposedValue;
    private string _currentId = string.Empty;
    private SKBitmap? _currentBitmap = null;
    private PixelImage? _source;

    public Rect Bounds { get; set; }

    public PixelImage? Source => _source;

    public void UpdateBitmap(PixelImage? source)
    {
        _source = source;
    }

    public bool Equals(ICustomDrawOperation? other)
    {
        return other?.Equals(this) == true;
    }

    public bool HitTest(Avalonia.Point p)
    {
        return Bounds.Contains(p);
    }

    public void Render(ImmediateDrawingContext context)
    {
        if (Source != null && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
        {
            ISkiaSharpApiLease lease = leaseFeature.Lease();
            using (lease)
            {
                if (Source.RenderId != _currentId || SkiaUtils.IsNull(_currentBitmap))
                {
                    if (!SkiaUtils.IsNull(_currentBitmap))
                    {
                        _currentBitmap.Dispose();
                        _currentBitmap = null;
                    }

                    _currentBitmap = Source.Render();
                    _currentId = Source.RenderId;
                }

                if (!SkiaUtils.IsNull(_currentBitmap))
                {
                    lease.SkCanvas.DrawBitmapLock(_currentBitmap, Bounds);
                }
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}