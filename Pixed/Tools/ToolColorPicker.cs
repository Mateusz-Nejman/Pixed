using Pixed.Input;
using Pixed.Models;
using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolColorPicker : BaseTool
    {
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            if (frame.ContainsPixel(x, y))
            {
                var color = frame.GetPixel(x, y);

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    Subjects.PrimaryColorChange.OnNext(color);
                }
                else if (Mouse.RightButton == MouseButtonState.Pressed)
                {
                    Subjects.SecondaryColorChange.OnNext(color);
                }
            }
        }
    }
}
