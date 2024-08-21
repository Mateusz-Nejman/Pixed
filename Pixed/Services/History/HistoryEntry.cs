namespace Pixed.Services.History
{
    internal struct HistoryEntry
    {
        public int[] PixelX { get; }
        public int[] PixelY { get; }
        public int[] OldColor { get; }
        public int[] NewColor { get; }
        public string FrameId { get; }
        public string LayerId { get; }

        public HistoryEntry(int[] pixelX, int[] pixelY, int[] oldColor, int[] newColor, string frameId, string layerId)
        {
            PixelX = pixelX;
            PixelY = pixelY;
            OldColor = oldColor;
            NewColor = newColor;
            FrameId = frameId;
            LayerId = layerId;
        }
    }
}
