using Pixed.Models;
using Pixed.Selection;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Tools;
using System.Collections.ObjectModel;

namespace Pixed;

internal static class Global
{
    public static BaseTool? ToolSelected { get; set; }
}
