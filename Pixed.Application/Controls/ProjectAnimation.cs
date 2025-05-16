using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;
using System;
using System.Reactive.Linq;

namespace Pixed.Application.Controls;

internal class ProjectAnimation : Control
{
    //TODO restrict animation to smaller sizes
    class DrawOperation(Rect bounds, ApplicationData applicationData, int frameIndex) : ICustomDrawOperation
    {
        private readonly ApplicationData _applicationData = applicationData;
        private readonly double _width = applicationData.CurrentModel.Width;
        private readonly double _height = applicationData.CurrentModel.Height;
        private int _frameIndex = frameIndex;

        private string[] _frameIds = [];
        private SKBitmap[] _frameBitmaps = [];

        public Rect Bounds { get; private set; } = bounds;

        public bool HitTest(Avalonia.Point p) => false;
        public bool Equals(ICustomDrawOperation other) => false;

        public void Update(Rect bounds, int frameIndex)
        {
            Bounds = bounds;
            _frameIndex = frameIndex;
        }

        public void Dispose()
        {
            // No-op
        }
        public void Render(ImmediateDrawingContext context)
        {
            CheckAndResetRenders();
            if (_frameIndex >= _applicationData.CurrentModel.Frames.Count)
            {
                return;
            }

            var bitmap = _frameBitmaps[_frameIndex];

            if (SkiaUtils.IsNull(bitmap))
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

                    if (!SkiaUtils.IsNull(bitmap))
                    {
                        canvas.DrawBitmapLock(bitmap, new Rect(Bounds.X, Bounds.Y, Bounds.Width, height));
                    }
                }
            }
        }

        private void CheckAndResetRenders()
        {
            if (_applicationData.CurrentModel.Frames.Count != _frameIds.Length)
            {
                _frameIds = new string[_applicationData.CurrentModel.Frames.Count];

                foreach (var bitmap in _frameBitmaps)
                {
                    bitmap.Dispose();
                }

                _frameBitmaps = new SKBitmap[_applicationData.CurrentModel.Frames.Count];
            }

            for (int a = 0; a < _frameIds.Length; a++)
            {
                if (_applicationData.CurrentModel.Frames[a].RenderId != _frameIds[a])
                {
                    if (!SkiaUtils.IsNull(_frameBitmaps[a]))
                    {
                        _frameBitmaps[a].Dispose();
                        _frameBitmaps[a] = null;
                    }

                    _frameBitmaps[a] = _applicationData.CurrentModel.Frames[a].Render();
                    _frameIds[a] = _applicationData.CurrentModel.Frames[a].RenderId;
                }
            }
        }
    }

    private readonly ApplicationData _applicationData;
    private IDisposable _timer;
    private int _current = 0;
    private readonly DrawOperation _drawOperation;

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
        _drawOperation = new DrawOperation(new Rect(), _applicationData, _current);
    }

    public override void Render(DrawingContext context)
    {
        _drawOperation.Update(Bounds, _current);
        context.Custom(_drawOperation);
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    private void SetTimer(bool enabled)
    {
        _timer?.Dispose();

        if (enabled)
        {
            _timer = Observable.Interval(TimeSpan.FromSeconds(0.1d)).Subscribe(l =>
            {
                _current++;

                if (_current >= _applicationData.CurrentModel.Frames.Count)
                {
                    _current = 0;
                }
            });
        }
    }
}