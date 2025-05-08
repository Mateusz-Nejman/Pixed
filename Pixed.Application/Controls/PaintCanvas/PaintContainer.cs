using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
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

    private readonly PaintContainerOperation _image = new();

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

        _image.UpdateBitmap(Source);

        Size sourceSize = new(Source.Width, Source.Height);
        Rect sourceRect = new(sourceSize);
        _image.Bounds = sourceRect;
        context.DrawImage(_image, sourceRect, sourceRect);
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
            result = Stretch.Uniform.CalculateSize(availableSize, new Size(Source.Width, Source.Height), StretchDirection.Both);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Source != null)
        {
            var sourceSize = new Size(Source.Width, Source.Height);
            var result = Stretch.Uniform.CalculateSize(finalSize, sourceSize);
            return result;
        }

        return new Size();
    }
}