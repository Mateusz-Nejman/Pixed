﻿using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolDithering : ToolPen
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _prevX = x;
            _prevY = y;

            bool usePrimary = (x + y) % 2 != 0;

            if(Mouse.LeftButton == MouseButtonState.Pressed)
            {
                usePrimary = !usePrimary;
            }

            var color = usePrimary ? Global.PrimaryColor : Global.SecondaryColor;

            DrawOnOverlay(color, x, y, frame, ref overlay);
        }
    }
}