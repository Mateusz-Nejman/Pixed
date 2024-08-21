using System.Drawing;

namespace Pixed.Tools
{
    internal class ToolEraser : ToolPen
    {
        public override Color GetToolColor()
        {
            return Color.Transparent;
        }
    }
}
