using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools
{
    internal class ToolPen : BaseTool
    {
        private int _previousX = -1;
        private int _previousY = -1;
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _previousX = x;
            _previousY = y;
            frame.SetPixel(x, y, GetToolColor().ToArgb());
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            this.ApplyTool(x, y, frame, ref overlay);

            _previousX = x;
            _previousY = y;
        }
    }
}
