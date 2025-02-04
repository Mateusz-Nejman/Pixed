using Pixed.Core.Models;
using SkiaSharp;

namespace Pixed.Application.Models;
internal class RenderModel : PixelImage
{
    private Frame? _frame;
    private SKBitmap? _overlay;
    private bool _overlayNeedRender = false;
    public override bool NeedRender { get => CheckNeedRender() || base.NeedRender; protected set => base.NeedRender = value; }

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
            _overlayNeedRender = true;
        }
    }
    public override SKBitmap Render()
    {
        if(!NeedRender)
        {
            return base.Render();
        }

        SKBitmap image = new(_frame.Width, _frame.Height, true);
        SKCanvas canvas = new(image);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(_frame.Render(), SKPoint.Empty);
        canvas.DrawBitmap(_overlay, SKPoint.Empty);
        canvas.Dispose();
        _render = image;
        NeedRender = false;
        return image;
    }

    private bool CheckNeedRender()
    {
        return (_frame != null && _frame.NeedRender) || (_overlay != null && _overlayNeedRender);
    }
}
