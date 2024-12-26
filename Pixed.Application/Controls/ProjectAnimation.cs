using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Platform;
using Pixed.Core.Models;
using SkiaSharp;
using Pixed.Application.Utils;
using System;
using Pixed.Common;
using System.Reactive.Linq;

namespace Pixed.Application.Controls;

internal class ProjectAnimation : Control
{
    class DrawOperation(Rect bounds, ApplicationData applicationData, int frameIndex) : ICustomDrawOperation
    {
        private readonly ApplicationData _applicationData = applicationData;
        private readonly double _width = applicationData.CurrentModel.Width;
        private readonly double _height = applicationData.CurrentModel.Height;
        private readonly int _frameIndex = frameIndex;

        public Rect Bounds { get; } = bounds;

        public bool HitTest(Avalonia.Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;

        public void Dispose()
        {
            // No-op
        }
        public void Render(ImmediateDrawingContext context)
        {
            var frameBitmap = _applicationData.CurrentModel.Frames[_frameIndex] ?? null;
            if(frameBitmap == null || frameBitmap.RenderSource.Source == null)
            {
                return;
            }
            if (context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    var canvas = lease.SkCanvas;
                    double ratio = Bounds.Width / _width;
                    double height = _height * ratio;
                    canvas.DrawBitmap(frameBitmap.RenderSource.Source, SKRect.Create(Bounds.X.ToFloat(), Bounds.Y.ToFloat(), Bounds.Width.ToFloat(), height.ToFloat()));
                }
            }
        }
    }

    private readonly ApplicationData _applicationData;
    private IDisposable _timer;
    private int _current = 0;

    public new bool IsVisible
    {
        get => base.IsVisible;
        set
        {
            base.IsVisible = value;
            SetTimer(value);
        }
    }

    public ProjectAnimation() : base()
    {
        _applicationData = App.ServiceProvider.Get<ApplicationData>();
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new DrawOperation(new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height), _applicationData, _current));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    private void SetTimer(bool enabled)
    {
        _timer?.Dispose();

        if(enabled)
        {
            _timer = Observable.Interval(TimeSpan.FromSeconds(0.1d)).Subscribe(l =>
            {
                _current++;

                if(_current >= _applicationData.CurrentModel.Frames.Count)
                {
                    _current = 0;
                }
            });
        }
    }
}