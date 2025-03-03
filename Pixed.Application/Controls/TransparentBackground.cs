using Pixed.Application.Utils;
using Pixed.Core;
using SkiaSharp;
using System;

namespace Pixed.Application.Controls;
internal class TransparentBackground : OverlayControl
{
    public TransparentBackground()
    {
        ClipToBounds = true;
    }
    public override void Render(SKCanvas canvas)
    {
        SKPaint paint1 = new() { Color = new UniColor(76, 76, 76), Style = SKPaintStyle.Fill };
        SKPaint paint2 = new() { Color = new UniColor(85, 85, 85), Style = SKPaintStyle.Fill };

        float right = Bounds.Right.ToFloat();
        float bottom = Bounds.Bottom.ToFloat();
        int xCount = ((int)right / 32)+1;
        int yCount = ((int)bottom / 32)+1;
        float size = 32f / Zoom.ToFloat();

        for(int x = 0; x < xCount; x++)
        {
            for(int y = 0; y < yCount; y++)
            {
                int c = (x + y) % 2;
                float cellRight = (x.ToFloat() * size) + size;
                float cellBottom = (y.ToFloat() * size) + size;
                canvas.DrawRect(new SKRect(x.ToFloat() * size, y.ToFloat() * size, MathF.Min(cellRight, right / Zoom.ToFloat()), MathF.Min(cellBottom, bottom / Zoom.ToFloat())), c == 0 ? paint1 : paint2);
            }
        }
    }
}
