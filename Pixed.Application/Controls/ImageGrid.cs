using Avalonia;
using Pixed.Application.Utils;
using Pixed.Core;
using SkiaSharp;
using System;


namespace Pixed.Application.Controls;
internal class ImageGrid : OverlayControl
{
    public const int MinGridSize = 15;
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
            float leftBounds = Bounds.Left.ToFloat() / Zoom.ToFloat();
            float topBounds = Bounds.Top.ToFloat() / Zoom.ToFloat();
            float rightBounds = Bounds.Right.ToFloat() / Zoom.ToFloat();
            float bottomBounds = Bounds.Bottom.ToFloat() / Zoom.ToFloat();
            float firstStepX = stepX;
            float firstStepY = stepY;
            float left = leftBounds;
            float top = topBounds;
            float right = rightBounds;
            float bottom = bottomBounds;

            if (VisualToZoomMatrix.HasValue && ZoomBorder != null)
            {
                var inverted = VisualToZoomMatrix.Value.Invert();
                var gridSize = new Point(GridWidth * Zoom, GridHeight * Zoom);

                if(gridSize.X < MinGridSize | gridSize.Y < MinGridSize)
                {
                    return;
                }
                var firstTransform = inverted.Transform(new Point(0, 0));
                var lastTransform = inverted.Transform(new Point(ZoomBorder.Bounds.Size.Width, ZoomBorder.Bounds.Size.Height));

                left = Math.Max(left, Math.Round(firstTransform.X).ToFloat() - 1f);
                top = Math.Max(top, Math.Round(firstTransform.Y).ToFloat() - 1f);
                bottom = Math.Min(bottomBounds, Math.Round(lastTransform.Y).ToFloat() + 1f);
                right = Math.Min(rightBounds, Math.Round(lastTransform.X).ToFloat() + 1f);
                firstStepX = ((int)left / (int)stepX) * stepX;
                firstStepY = ((int)top / (int)stepY) * stepY;
            }

            for (float x = firstStepX; x < (int)right; x += stepX)
            {
                canvas.DrawLine(new SKPoint(leftBounds + x, topBounds), new SKPoint(leftBounds + x, bottomBounds), new SKPaint() { Color = GridColor });
            }
            for (float y = firstStepY; y < MathF.Floor(bottom); y += stepY)
            {
                canvas.DrawLine(new SKPoint(leftBounds, topBounds + y), new SKPoint(rightBounds, topBounds + y), new SKPaint() { Color = GridColor });
            }
        }
    }
}
