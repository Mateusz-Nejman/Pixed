using Pixed.Core.Models;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Application.Models;
internal class RenderModel : PixelImage
{
    private Frame? _frame;
    private SKBitmap? _overlay;

    public override int Width => _frame?.Width ?? 0;
    public override int Height => _frame?.Height ?? 0;

    public override string RenderId 
    { 
        get => _frame.RenderId;
        set
        {
            _frame.RenderId = value;
        }
    }

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
        if (!SkiaUtils.IsNull(_overlay))
        {
            canvas.DrawBitmap(_overlay, SKPoint.Empty);
        }
        canvas.Dispose();
        return image;
    }
}
