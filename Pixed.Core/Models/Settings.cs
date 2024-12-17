namespace Pixed.Core.Models;

public class Settings
{
    public int UserWidth { get; set; } = 16;
    public int UserHeight { get; set; } = 16;
    public UniColor GridColor { get; set; } = UniColor.Black;
    public int GridWidth { get; set; } = 1;
    public int GridHeight { get; set; } = 1;
    public bool GridEnabled { get; set; } = true;
    public bool MaintainAspectRatio { get; set; } = false;
}
