using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
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
internal class PixelImageControl : Control
{
    private const int TICK_TIME = 50;
    private class InternalImage : IImage, ICustomDrawOperation
    {
        private SKBitmap? _source;
        private Size _size;

        public Size Size => _size;

        public Rect Bounds { get; set; }

        public SKBitmap? Source => _source;

        public InternalImage(SKBitmap? source)
        {
            UpdateBitmap(source);
        }

        public void UpdateBitmap(SKBitmap? source)
        {
            _source = source;
            if (source?.Info.Size is SKSizeI size)
            {
                _size = new(size.Width, size.Height);
            }
        }

        public InternalImage Clone()
        {
            return new InternalImage(_source?.Copy());
        }

        public InternalImage(uint[] colors, Pixed.Core.Models.Point size) : this(SkiaUtils.FromArray(colors, size))
        {
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
            if (Source is SKBitmap bitmap && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    lease.SkCanvas.DrawBitmap(bitmap, Bounds);
                }
            }
        }

        public void Dispose()
        {
        }
    }
    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<PixelImage?> SourceProperty =
        AvaloniaProperty.Register<PixelImageControl, PixelImage?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Stretch"/> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<PixelImageControl, Stretch>(nameof(Stretch), Stretch.Uniform);

    /// <summary>
    /// Defines the <see cref="StretchDirection"/> property.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<PixelImageControl, StretchDirection>(
            nameof(StretchDirection),
            StretchDirection.Both);

    static PixelImageControl()
    {
        AffectsRender<PixelImageControl>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AffectsMeasure<PixelImageControl>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<PixelImageControl>(AutomationControlType.Image);
    }

    private readonly InternalImage _image = new(null);
    private IDisposable _refreshTick;

    /// <summary>
    /// Gets or sets the image that will be displayed.
    /// </summary>
    [Content]
    public PixelImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value controlling how the image will be stretched.
    /// </summary>
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Gets or sets a value controlling in what direction the image will be stretched.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    /// <inheritdoc />
    protected override bool BypassFlowDirectionPolicies => true;

    /// <summary>
    /// Renders the control.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public sealed override void Render(DrawingContext context)
    {
        var source = Source?.Render();
        _image.UpdateBitmap(source);

        if (source != null && Bounds.Width > 0 && Bounds.Height > 0)
        {
            Rect viewPort = new(Bounds.Size);
            Size sourceSize = new(source.Width, source.Height);

            Vector scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
            Size scaledSize = sourceSize * scale;
            Rect destRect = viewPort
                .CenterRect(new Rect(scaledSize))
                .Intersect(viewPort);
            Rect sourceRect = new Rect(sourceSize)
                .CenterRect(new Rect(destRect.Size / scale));

            _image.Bounds = destRect;
            context.DrawImage(_image, sourceRect, destRect);
        }
    }

    /// <summary>
    /// Measures the control.
    /// </summary>
    /// <param name="availableSize">The available size.</param>
    /// <returns>The desired size of the control.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        var source = Source?.Render();
        var result = new Size();

        if (source != null)
        {
            result = Stretch.CalculateSize(availableSize, new Size(source.Width, source.Height), StretchDirection);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var source = Source?.Render();

        if (source != null)
        {
            var sourceSize = new Size(source.Width, source.Height);
            var result = Stretch.CalculateSize(finalSize, sourceSize);
            return result;
        }
        else
        {
            return new Size();
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ImageAutomationPeer(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        _refreshTick ??= Observable.Interval(TimeSpan.FromMilliseconds(TICK_TIME)).Subscribe(t =>
            {
                Dispatcher.UIThread.Invoke(() => _image?.UpdateBitmap(Source?.Render()));
            });
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _refreshTick?.Dispose();
    }
}