namespace Pixed.Services.History;

internal readonly struct HistoryEntry(int[] pixelX, int[] pixelY, int[] oldColor, int[] newColor, string frameId, string layerId)
{
    public int[] PixelX { get; } = pixelX;
    public int[] PixelY { get; } = pixelY;
    public int[] OldColor { get; } = oldColor;
    public int[] NewColor { get; } = newColor;
    public string FrameId { get; } = frameId;
    public string LayerId { get; } = layerId;
}
