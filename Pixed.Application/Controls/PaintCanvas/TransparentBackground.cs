using Avalonia.Interactivity;
using Pixed.Application.Utils;
using Pixed.Core;
using Pixed.Core.Utils;
using SkiaSharp;

namespace Pixed.Application.Controls.PaintCanvas;
internal class TransparentBackground : OverlayControl
{
    private const int MIN_ZOOM = 8;

    private readonly UniColor _color1 = new(76, 76, 76);
    private readonly UniColor _color2 = new(85, 85, 85);

    private readonly SKPaint _paint;
    private readonly SKPaint _solidPaint;
    public TransparentBackground()
    {
        ClipToBounds = false;
        var background = CreateTransparentBackground();
        var shader = SKShader.CreateBitmap(background, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
        _paint = new SKPaint()
        {
            Shader = shader,
        };

        _solidPaint = new SKPaint()
        {
            Color = _color2
        };
        background.Dispose();
        shader.Dispose();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _paint.Dispose();
    }

    public override void Render(SKCanvas canvas)
    {
        canvas.DrawRect(SKRect.Create(Bounds.Width.ToFloat() / Zoom.ToFloat(), Bounds.Height.ToFloat() / Zoom.ToFloat()), Zoom >= MIN_ZOOM ? _paint : _solidPaint);
    }

    private SKBitmap CreateTransparentBackground()
    {
        SKBitmap bitmap = SkiaUtils.GetBitmap(2, 2);
        bitmap.SetPixel(0, 0, _color1);
        bitmap.SetPixel(1, 0, _color2);
        bitmap.SetPixel(1, 1, _color1);
        bitmap.SetPixel(0, 1, _color2);
        return bitmap;
    }
}
