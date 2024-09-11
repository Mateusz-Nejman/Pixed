using System.Collections.Generic;

namespace Pixed.Services.History
{
    internal class DynamicHistoryEntry
    {
        public List<int> PixelX { get; set; }
        public List<int> PixelY { get; }
        public List<int> OldColor { get; }
        public List<int> NewColor { get; }
        public string FrameId { get; set; }
        public string LayerId { get; set; }

        public DynamicHistoryEntry()
        {
            PixelX = [];
            PixelY = [];
            OldColor = [];
            NewColor = [];
            FrameId = string.Empty;
            LayerId = string.Empty;
        }

        public void Add(int x, int y, int oldColor, int endColor)
        {
            PixelX.Add(x);
            PixelY.Add(y);
            OldColor.Add(oldColor);
            NewColor.Add(endColor);
        }

        public HistoryEntry ToEntry()
        {
            return new HistoryEntry(PixelX.ToArray(), PixelY.ToArray(), OldColor.ToArray(), NewColor.ToArray(), FrameId, LayerId);
        }
    }
}
