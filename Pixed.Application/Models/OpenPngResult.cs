namespace Pixed.Application.Models;

internal readonly struct OpenPngResult(int tileWidth, int tileHeight, bool isTileset)
{
    public int TileWidth { get; } = tileWidth;
    public int TileHeight { get; } = tileHeight;
    public bool IsTileset { get; } = isTileset;
}