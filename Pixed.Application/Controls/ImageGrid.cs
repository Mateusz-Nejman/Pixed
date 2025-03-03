using Pixed.Application.Utils;
using Pixed.Core;
using SkiaSharp;


namespace Pixed.Application.Controls;
internal class ImageGrid : OverlayControl
{
    public ImageGrid()
    {
        ClipToBounds = false;
    }
    public double GridWidth { get; set; }
    public double GridHeight { get; set; }
    public bool GridEnabled { get; set; } = false;
    public UniColor GridColor { get; set; } = UniColor.Black;

    public override void Render(SKCanvas canvas)
    {
        if (GridEnabled)
        {
            float stepX = GridWidth.ToFloat();
            float stepY = GridHeight.ToFloat();
            float left = Bounds.Left.ToFloat();
            float top = Bounds.Top.ToFloat();
            float right = Bounds.Right.ToFloat() / Zoom.ToFloat();
            float bottom = Bounds.Bottom.ToFloat() / Zoom.ToFloat();

            for (float x = stepX; x < (int)right; x += stepX)
            {
                canvas.DrawLine(new SKPoint(left + x, top), new SKPoint(left + x, bottom), new SKPaint() { Color = GridColor });
            }
            for (float y = stepY; y < (int)bottom; y += stepY)
            {
                canvas.DrawLine(new SKPoint(left, top + y), new SKPoint(right, top + y), new SKPaint() { Color = GridColor });
            }
        }
    }
}
