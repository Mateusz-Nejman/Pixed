using Pixed.Input;
using Pixed.Models;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolOutliner : BaseTool
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            bool fillCorners = Keyboard.Modifiers.HasFlag(Avalonia.Input.KeyModifiers.Control);
            var color = GetToolColor();
            PaintUtils.OutlineSimiliarConnectedPixels(frame.CurrentLayer, x, y, color, fillCorners);
        }
    }
}
