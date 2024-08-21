using Pixed.Models;
using Pixed.Services.History;
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
        private DynamicHistoryEntry _historyEntry = new();
        public override void ApplyTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _historyEntry.FrameId = frame.Id;
            _historyEntry.LayerId = frame.Layers[frame.SelectedLayer].Id;
            _historyEntry.PixelX.Add(x);
            _historyEntry.PixelY.Add(y);
            _historyEntry.OldColor.Add(frame.GetPixel(x, y));
            _historyEntry.NewColor.Add(GetToolColor().ToArgb());
            frame.SetPixel(x, y, GetToolColor().ToArgb());
            Subjects.RefreshCanvas.OnNext(true);
        }

        public override void MoveTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            this.ApplyTool(x, y, frame, ref overlay);
        }

        public override void ReleaseTool(int x, int y, Frame frame, ref Bitmap overlay)
        {
            base.ReleaseTool(x, y, frame, ref overlay);
            Global.Models[0].AddHistory(_historyEntry.ToEntry());
            _historyEntry = new DynamicHistoryEntry();
        }
    }
}
