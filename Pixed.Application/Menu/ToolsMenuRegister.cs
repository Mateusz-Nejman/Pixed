using Pixed.Common.Menu;
using Pixed.Common.Tools;
using Pixed.Core;

namespace Pixed.Application.Menu;
internal class ToolsMenuRegister(IMenuItemRegistry menuItemRegistry, ToolSelector toolSelector)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ToolSelector _toolSelector = toolSelector;

    public void Register()
    {
        ActionCommand<string> selectToolAction = new(_toolSelector.SelectTool);
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Pen tool", selectToolAction, "tool_pen");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Vertical mirror pen", selectToolAction, "tool_mirror_pen");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Paint bucket tool", selectToolAction, "tool_paint_bucket");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Paint all pixels of the same color", selectToolAction, "tool_colorswap");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Eraser tool", selectToolAction, "tool_eraser");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Stroke tool", selectToolAction, "tool_stroke");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Rectangle tool", selectToolAction, "tool_rectangle");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Circle tool", selectToolAction, "tool_circle");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Move tool", selectToolAction, "tool_move");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Shape selection", selectToolAction, "tool_shape_select");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Rectangle selection", selectToolAction, "tool_rectangle_select");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Lasso selection", selectToolAction, "tool_lasso_select");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Lighten", selectToolAction, "tool_lighten");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Dithering tool", selectToolAction, "tool_dithering");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Colorpicker", selectToolAction, "tool_colorpicker");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Pixelart noise tool", selectToolAction, "tool_noise");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Pixelart noise fill tool", selectToolAction, "tool_noise_fill");
        _menuItemRegistry.Register(BaseMenuItem.Tools, "Outliner tool", selectToolAction, "tool_outliner_tool");
    }
}
