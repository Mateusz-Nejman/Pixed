using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Pixed.Core.Models;
using SkiaSharp;

namespace Pixed.Application.Controls.PaintCanvas;

internal class PaintContainer : Control
{
    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<PixelImage?> SourceProperty =
        AvaloniaProperty.Register<PaintContainer, PixelImage?>(nameof(Source));


    static PaintContainer()
    {
        AffectsRender<PaintContainer>(SourceProperty);
        AffectsMeasure<PaintContainer>(SourceProperty);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<PaintContainer>(AutomationControlType.Image);
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

        var source = Source.Render();

        if (source == null)
        {
            return;
        }
        _image.UpdateBitmap(Source);

        if (source != null)
        {
            Size sourceSize = new(source.Width, source.Height);
            Rect sourceRect = new(sourceSize);
            _image.Bounds = sourceRect;
            context.DrawImage(_image, sourceRect, sourceRect);
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
            result = Stretch.Uniform.CalculateSize(availableSize, new Size(source.Width, source.Height), StretchDirection.Both);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Source?.Render() is SKBitmap source)
        {
            var sourceSize = new Size(source.Width, source.Height);
            var result = Stretch.Uniform.CalculateSize(finalSize, sourceSize);
            return result;
        }

        return new Size();
    }
}