﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace Pixed.Application.Controls;
internal abstract class OverlayControl : Control
{
    class DrawOperation(Rect bounds, OverlayControl instance) : ICustomDrawOperation
    {
        private readonly OverlayControl _instance = instance;

        public Rect Bounds { get; } = bounds;
        public bool HitTest(Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;

        public void Dispose()
        {
            // No-op
        }
        public void Render(ImmediateDrawingContext context)
        {
            if (context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    _instance.Render(lease.SkCanvas);
                }
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new DrawOperation(new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height), this));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    public abstract void Render(SKCanvas canvas);
}