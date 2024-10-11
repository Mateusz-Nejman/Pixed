using Pixed.Models;
using Pixed.Utils;
using System;

namespace Pixed.Tools;
internal class ToolCircle(ApplicationData applicationData) : ShapeTool(applicationData)
{
    protected override void Draw(int x, int y, uint color, bool isShift, Action<int, int, uint> setPixelAction)
    {
        var rectangle = MathUtils.GetOrderedRectangle(_startX, _startY, x, y);

        if (isShift)
        {
            int width = Math.Abs(rectangle[2] - rectangle[0]);
            int height = Math.Abs(rectangle[3] - rectangle[1]);
            int size = Math.Min(width, height);

            rectangle[2] = rectangle[0] + size;
            rectangle[3] = rectangle[1] + size;
        }

        var circle = MathUtils.GetCircle(rectangle[0], rectangle[1], rectangle[2], rectangle[3]);

        foreach (var point in circle)
        {
            setPixelAction?.Invoke(point.X, point.Y, color);
        }
    }
}
