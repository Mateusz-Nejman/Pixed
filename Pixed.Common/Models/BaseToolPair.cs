using Pixed.Tools;

namespace Pixed.Models;
internal readonly struct BaseToolPair(BaseTool prev, BaseTool current)
{
    public BaseTool Previous { get; } = prev;
    public BaseTool Current { get; } = current;
}
