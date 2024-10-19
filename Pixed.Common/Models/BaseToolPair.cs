using Pixed.Common.Tools;

namespace Pixed.Common.Models;
public readonly struct BaseToolPair(BaseTool prev, BaseTool current)
{
    public BaseTool Previous { get; } = prev;
    public BaseTool Current { get; } = current;
}
