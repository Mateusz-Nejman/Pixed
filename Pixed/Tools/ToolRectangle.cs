using Pixed.Models;
using Pixed.Services.History;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools
{
    internal class ToolRectangle : ShapeTool
    {
        protected override void Draw(int x, int y, int color, bool isShift, Action<int, int, int> setPixelAction)
        {
            var rectangle = MathUtils.GetOrderedRectangle(_startX, _startY, x, y);

            if(isShift)
            {
                int width = Math.Abs(rectangle[2] - rectangle[0]);
                int height = Math.Abs(rectangle[3] - rectangle[1]);
                int size = Math.Min(width, height);

                rectangle[2] = rectangle[0] + size;
                rectangle[3] = rectangle[1] + size;
            }

            for(int rx = rectangle[0]; rx <= rectangle[2]; rx++)
            {
                for(int ry = rectangle[1]; ry <= rectangle[3]; ry++)
                {
                    if(rx == rectangle[0] || rx == rectangle[2] || ry == rectangle[1] || ry == rectangle[3])
                    {
                        setPixelAction.Invoke(rx, ry, color);
                    }
                }
            }
        }
    }
}
