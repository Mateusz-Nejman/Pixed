using Pixed.Core.Models;
using SkiaSharp;

namespace Pixed.Application.Models;
internal class RenderModel : PixelImage
{
    private Frame? _frame;
    private SKBitmap? _overlay;

    public Frame? Frame
    {
        get => _frame;
        set
        {
            _frame = value;
        }
    }
    public SKBitmap? Overlay
    {
        get => _overlay;
        set
        {
            _overlay = value;
        }
    }
    public override SKBitmap Render()
    {
        SKBitmap image = new(_frame.Width, _frame.Height, true);
        SKCanvas canvas = new(image);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(_frame.Render(), SKPoint.Empty);
        if(_overlay != null)
        {
            canvas.DrawBitmap(_overlay, SKPoint.Empty);
        }
        canvas.Dispose();
        return image;
    }
}
