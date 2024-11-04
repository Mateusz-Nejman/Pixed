using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Pixed.Common;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Data;


namespace Pixed.Application.Controls;
internal class ImageGrid : Control
{
    public ImageGrid()
    {
        ClipToBounds = false;
    }
    public double Zoom { get; set; } = 1;
    public double GridWidth { get; set; }
    public double GridHeight { get; set; }
    public bool GridEnabled { get; set; } = false;
    public UniColor GridColor { get; set; } = UniColor.Black;

    class CustomDrawOp : ICustomDrawOperation
    {
        private readonly float _zoom;
        private readonly float _left;
        private readonly float _top;
        private readonly float _right;
        private readonly float _bottom;
        private readonly float _gridWidth;
        private readonly float _gridHeight;
        private readonly bool _gridEnabled;
        private readonly SKColor _gridColor;
        public CustomDrawOp(Rect bounds, float zoom, float gridWidth, float gridHeight, bool gridEnabled, SKColor gridColor)
        {
            Bounds = bounds;
            _zoom = zoom;
            _left = (float)bounds.Left / _zoom;
            _top = (float)bounds.Top / _zoom;
            _right = (float)bounds.Width / _zoom;
            _bottom = (float)bounds.Height / _zoom;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _gridEnabled = gridEnabled;
            _gridColor = gridColor;
        }

        public void Dispose()
        {
            // No-op
        }

        public Rect Bounds { get; }
        public bool HitTest(Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;
        public void Render(ImmediateDrawingContext context)
        {
            if (context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    if(_gridEnabled)
                    {
                        float stepX = _gridWidth;
                        float stepY = _gridHeight;

                        for(float x = stepX; x < (int)_right; x += stepX)
                        {
                            lease.SkCanvas.DrawLine(new SKPoint(_left + x, _top), new SKPoint(_left + x, _bottom), new SKPaint() { Color = _gridColor });
                        }
                        for (float y = stepY; y < (int)_bottom; y += stepY)
                        {
                            lease.SkCanvas.DrawLine(new SKPoint(_left, _top + y), new SKPoint(_right, _top + y), new SKPaint() { Color = _gridColor });
                        }
                    }
                }
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new CustomDrawOp(new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height), (float)Zoom, (float)GridWidth, (float)GridHeight, GridEnabled, GridColor));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}
