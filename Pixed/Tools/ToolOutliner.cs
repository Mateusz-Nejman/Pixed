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
            var entry = PaintUtils.OutlineSimiliarConnectedPixels(frame.Layers[frame.SelectedLayer], x, y, color, fillCorners);
            entry.FrameId = frame.Id;
            Global.CurrentModel.AddHistory(entry.ToEntry());
            Subjects.RefreshCanvas.OnNext(entry);
        }
    }
}
