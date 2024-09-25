using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Tools;
using System;
using System.Windows.Input;
using static Pixed.MenuBuilder;

namespace Pixed.ViewModels
{
    internal class ToolsSectionViewModel : PixedViewModel
    {
        private readonly MenuBuilder _menuBuilder;
        public ICommand ToolSelectAction { get; }

        public ToolsSectionViewModel(MenuBuilder menuBuilder, ToolSelector toolSelector)
        {
            _menuBuilder = menuBuilder;
            ToolSelectAction = new ActionCommand<string>(name =>
            {
                toolSelector.SelectTool(name);
            });
        }

        public override void RegisterMenuItems()
        {
            RegisterMenuItem(BaseMenuItem.Tools, "Pen tool", ToolSelectAction, "tool_pen");
            RegisterMenuItem(BaseMenuItem.Tools, "Vertical mirror pen", ToolSelectAction, "tool_mirror_pen");
            RegisterMenuItem(BaseMenuItem.Tools, "Paint bucket tool", ToolSelectAction, "tool_paint_bucket");
            RegisterMenuItem(BaseMenuItem.Tools, "Paint all pixels of the same color", ToolSelectAction, "tool_colorswap");
            RegisterMenuItem(BaseMenuItem.Tools, "Eraser tool", ToolSelectAction, "tool_eraser");
            RegisterMenuItem(BaseMenuItem.Tools, "Stroke tool", ToolSelectAction, "tool_stroke");
            RegisterMenuItem(BaseMenuItem.Tools, "Rectangle tool", ToolSelectAction, "tool_rectangle");
            RegisterMenuItem(BaseMenuItem.Tools, "Circle tool", ToolSelectAction, "tool_circle");
            RegisterMenuItem(BaseMenuItem.Tools, "Move tool", ToolSelectAction, "tool_move");
            RegisterMenuItem(BaseMenuItem.Tools, "Shape selection", ToolSelectAction, "tool_shape_select");
            RegisterMenuItem(BaseMenuItem.Tools, "Rectangle selection", ToolSelectAction, "tool_rectangle_select");
            RegisterMenuItem(BaseMenuItem.Tools, "Lasso selection", ToolSelectAction, "tool_lasso_select");
            RegisterMenuItem(BaseMenuItem.Tools, "Lighten", ToolSelectAction, "tool_lighten");
            RegisterMenuItem(BaseMenuItem.Tools, "Dithering tool", ToolSelectAction, "tool_dithering");
            RegisterMenuItem(BaseMenuItem.Tools, "Colorpicker", ToolSelectAction, "tool_colorpicker");
            RegisterMenuItem(BaseMenuItem.Tools, "Pixelart noise tool", ToolSelectAction, "tool_noise");
            RegisterMenuItem(BaseMenuItem.Tools, "Pixelart noise fill tool", ToolSelectAction, "tool_noise_fill");
            RegisterMenuItem(BaseMenuItem.Tools, "Outliner tool", ToolSelectAction, "tool_outliner_tool");
        }

        private void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
        {
            RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
        }

        private void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            _menuBuilder.AddEntry(baseMenu, menuItem);
        }
    }
}
