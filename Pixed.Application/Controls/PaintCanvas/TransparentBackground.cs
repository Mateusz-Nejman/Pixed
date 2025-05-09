using Avalonia.Interactivity;
using Pixed.Application.Utils;
using Pixed.Core;
using SkiaSharp;

namespace Pixed.Application.Controls.PaintCanvas;
internal class TransparentBackground : OverlayControl
{
    private readonly SKPaint _paint;
    public TransparentBackground()
    {
        ClipToBounds = false;
        var background = CreateTransparentBackground();
        var shader = SKShader.CreateBitmap(background, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
        _paint = new SKPaint()
        {
            Shader = shader,
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
        canvas.DrawRect(SKRect.Create(Bounds.Width.ToFloat() / Zoom.ToFloat(), Bounds.Height.ToFloat() / Zoom.ToFloat()), _paint);
    }

    private static SKBitmap CreateTransparentBackground()
    {
        UniColor color1 = new(76, 76, 76);
        UniColor color2 = new(85, 85, 85);
        SKBitmap bitmap = new(2, 2);
        bitmap.SetPixel(0, 0, color1);
        bitmap.SetPixel(1, 0, color2);
        bitmap.SetPixel(1, 1, color1);
        bitmap.SetPixel(0, 1, color2);
        return bitmap;
    }
}
