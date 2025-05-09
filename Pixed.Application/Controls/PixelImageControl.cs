using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Application.Controls;
internal class PixelImageControl : Control
{
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

    private readonly PixelDrawOperation _image = new();

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
        if (Source == null)
        {
            return;
        }

        _image.UpdateBitmap(Source);

        if (Bounds.Width > 0 && Bounds.Height > 0)
        {
            Rect viewPort = new(Bounds.Size);
            Size sourceSize = new(Source.Width, Source.Height);

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

        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
    }

    /// <summary>
    /// Measures the control.
    /// </summary>
    /// <param name="availableSize">The available size.</param>
    /// <returns>The desired size of the control.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        var result = new Size();

        if (Source != null)
        {
            result = Stretch.CalculateSize(availableSize, new Size(Source.Width, Source.Height), StretchDirection);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Source != null)
        {
            var sourceSize = new Size(Source.Width, Source.Height);
            var result = Stretch.CalculateSize(finalSize, sourceSize);
            return result;
        }
        else
        {
            return new Size();
        }
    }
}