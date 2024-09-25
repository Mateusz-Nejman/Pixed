using Pixed.Controls;
using Pixed.Menu;
using Pixed.Tools;
using System.Windows.Input;
using static Pixed.Menu.MenuBuilder;

namespace Pixed.ViewModels
{
    internal class ToolsSectionViewModel(MenuItemRegistry menuItemRegistry, ToolSelector toolSelector) : PixedViewModel
    {
        private readonly MenuItemRegistry _menuItemRegistry = menuItemRegistry;
        public ICommand ToolSelectAction { get; } = new ActionCommand<string>(name =>
            {
                toolSelector.SelectTool(name);
            });

        public override void RegisterMenuItems()
        {
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Pen tool", ToolSelectAction, "tool_pen");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Vertical mirror pen", ToolSelectAction, "tool_mirror_pen");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Paint bucket tool", ToolSelectAction, "tool_paint_bucket");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Paint all pixels of the same color", ToolSelectAction, "tool_colorswap");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Eraser tool", ToolSelectAction, "tool_eraser");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Stroke tool", ToolSelectAction, "tool_stroke");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Rectangle tool", ToolSelectAction, "tool_rectangle");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Circle tool", ToolSelectAction, "tool_circle");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Move tool", ToolSelectAction, "tool_move");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Shape selection", ToolSelectAction, "tool_shape_select");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Rectangle selection", ToolSelectAction, "tool_rectangle_select");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Lasso selection", ToolSelectAction, "tool_lasso_select");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Lighten", ToolSelectAction, "tool_lighten");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Dithering tool", ToolSelectAction, "tool_dithering");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Colorpicker", ToolSelectAction, "tool_colorpicker");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Pixelart noise tool", ToolSelectAction, "tool_noise");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Pixelart noise fill tool", ToolSelectAction, "tool_noise_fill");
            _menuItemRegistry.Register(BaseMenuItem.Tools, "Outliner tool", ToolSelectAction, "tool_outliner_tool");
        }
    }
}
