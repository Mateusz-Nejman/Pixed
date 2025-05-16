using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Pixed.Common;
using Pixed.Common.Tools;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System;

namespace Pixed.Application.Controls.PaintCanvas;

public class PaintContainerOperation : ICustomPixedImageOperation
{
    private bool _disposedValue;
    private PixelImage? _source;
    private string _currentId = string.Empty;
    private SKBitmap? _currentBitmap = null;
    private readonly IDisposable _toolChanged;
    private BaseTool? _tool;

    public Rect Bounds { get; set; }

    public PixelImage? Source => _source;

    public PaintContainerOperation() : this(null)
    {
    }
    public PaintContainerOperation(PixelImage? source)
    {
        _toolChanged = Subjects.ToolChanged.Subscribe(p =>
        {
            _tool = p.Current;
        });
        UpdateBitmap(source);
    }

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

                _tool?.OnOverlay(lease.SkCanvas);
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