using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Pixed.Common.Menu;
using Pixed.Common.Tools;
using Pixed.Core;
using System;
using System.Collections.Generic;

namespace Pixed.Application.Menu;
internal class ToolsMenuRegister(IMenuItemRegistry menuItemRegistry, ToolSelector toolSelector)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ToolSelector _toolSelector = toolSelector;
    private readonly Dictionary<string, string> _toolNames = new Dictionary<string, string>()
    {
        {"tool_pen", "Pen tool"},
        {"tool_mirror_pen", "Vertical mirror pen"},
        {"tool_paint_bucket", "Paint bucket tool"},
        {"tool_colorswap", "Paint all pixels of the same color"},
        {"tool_eraser", "Eraser tool"},
        {"tool_stroke", "Stroke tool"},
        {"tool_rectangle", "Rectangle tool"},
        {"tool_circle", "Circle tool"},
        {"tool_move", "Move tool"},
        {"tool_shape_select", "Shape selection"},
        {"tool_rectangle_select", "Rectangle selection"},
        {"tool_lasso_select", "Lasso selection"},
        {"tool_lighten", "Lighten"},
        {"tool_dithering", "Dithering tool"},
        {"tool_colorpicker", "Colorpicker"},
        {"tool_noise", "Pixelart noise tool"},
        {"tool_noise_fill", "Pixelart noise fill tool"},
        {"tool_outliner_tool", "Outliner tool"}
    };

    public void Register()
    {

        foreach(var tool in _toolSelector.GetTools())
        {
            RegisterTool(tool.Value, tool.Key);
        }
    }

    private void RegisterTool(BaseTool tool, string toolId)
    {
        ActionCommand<string> selectToolAction = new(_toolSelector.SelectTool);
        Uri uri = new(tool.ImagePath);
        Bitmap? icon = null;
        if (AssetLoader.Exists(uri))
        {
            var stream = AssetLoader.Open(uri);
            icon = new Bitmap(stream);
            stream.Dispose();
        }

        string name = _toolNames.TryGetValue(toolId, out string? value) ? value : toolId;
        _menuItemRegistry.Register(BaseMenuItem.Tools, name, selectToolAction, toolId, icon);
    }
}
