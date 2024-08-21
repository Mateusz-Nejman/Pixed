using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public HistoryEntry ToEntry()
        {
            return new HistoryEntry(PixelX.ToArray(), PixelY.ToArray(), OldColor.ToArray(), NewColor.ToArray(), FrameId, LayerId);
        }
    }
}
