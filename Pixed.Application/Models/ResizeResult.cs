using Pixed.Common.Utils;

namespace Pixed.Application.Models;
internal readonly struct ResizeResult(int width, int height, bool resizeCanvasContent, ResizeUtils.Origin anchor, bool maintainAspectRatio)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public bool ResizeCanvasContent { get; } = resizeCanvasContent;
    public ResizeUtils.Origin Anchor { get; } = anchor;
    public bool MaintainAspectRatio { get; } = maintainAspectRatio;
}