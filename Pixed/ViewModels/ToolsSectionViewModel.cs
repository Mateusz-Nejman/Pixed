using Pixed.Controls;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class ToolsSectionViewModel : PropertyChangedBase, IPixedViewModel
    {
        public ICommand ToolSelectAction { get; }

        public ToolsSectionViewModel()
        {
            ToolSelectAction = new ActionCommand<string>(name =>
            {
                if (Global.ToolSelector != null)
                {
                    Global.ToolSelector.SelectTool(name);
                }
            });
        }

        public void RegisterMenuItems()
        {
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Pen tool", ToolSelectAction, "tool_pen");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Vertical mirror pen", ToolSelectAction, "tool_mirror_pen");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Paint bucket tool", ToolSelectAction, "tool_paint_bucket");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Paint all pixels of the same color", ToolSelectAction, "tool_colorswap");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Eraser tool", ToolSelectAction, "tool_eraser");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Stroke tool", ToolSelectAction, "tool_stroke");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Rectangle tool", ToolSelectAction, "tool_rectangle");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Circle tool", ToolSelectAction, "tool_circle");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Move tool", ToolSelectAction, "tool_move");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Shape selection", ToolSelectAction, "tool_shape_select");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Rectangle selection", ToolSelectAction, "tool_rectangle_select");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Lasso selection", ToolSelectAction, "tool_lasso_select");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Lighten", ToolSelectAction, "tool_lighten");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Dithering tool", ToolSelectAction, "tool_dithering");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Colorpicker", ToolSelectAction, "tool_colorpicker");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Pixelart noise tool", ToolSelectAction, "tool_noise");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Pixelart noise fill tool", ToolSelectAction, "tool_noise_fill");
            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, "Outliner tool", ToolSelectAction, "tool_outliner_tool");
        }
    }
}
