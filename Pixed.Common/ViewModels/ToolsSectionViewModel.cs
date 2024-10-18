using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Pixed.Controls;
using Pixed.Menu;
using Pixed.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using static Pixed.Menu.MenuBuilder;

namespace Pixed.ViewModels;
internal class ToolsSectionViewModel(MenuItemRegistry menuItemRegistry, ToolSelector toolSelector, PaintCanvasViewModel paintCanvas) : PixedViewModel
{
    private readonly MenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ToolSelector _toolSelector = toolSelector;
    private readonly PaintCanvasViewModel _paintCanvas = paintCanvas;
    private readonly Dictionary<string, ToolRadioButton> _radios = [];
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

    public void InitializeTools(StackPanel stackPanel)
    {
        _toolSelector.SelectToolAction = SelectTool;
        _radios.Clear();
        var tools = _toolSelector.GetTools();

        foreach (var tool in tools)
        {
            bool hasCustomProperties = tool.Value.GetToolProperties().Count != 0;
            ToolRadioButton toolRadio = new()
            {
                Name = tool.Key
            };

            toolRadio.IsCheckedChanged += ToolRadioButton_IsCheckedChanged;
            toolRadio.PointerPressed += Radio_PointerPressed;
            toolRadio.Holding += Radio_Holding;

            if (Application.Current.TryFindResource(hasCustomProperties ? "ToolRadioCustom" : "ToolRadio", out var res) && res is ControlTheme theme)
            {
                toolRadio.Theme = theme;
            }

            if (AssetLoader.Exists(new Uri(tool.Value.ImagePath)))
            {
                var stream = AssetLoader.Open(new Uri(tool.Value.ImagePath));
                toolRadio.Source = new Bitmap(stream);
                stream.Dispose();
            }
            else
            {
                Console.WriteLine("Asset " + tool.Value.ImagePath + " not exists");
            }

            if (tool.Value.ToolTipProperties.HasValue)
            {
                ToolTip.SetTip(toolRadio, new ToolTooltip(tool.Value.ToolTipProperties.Value));
            }

            stackPanel.Children.Add(toolRadio);
            _radios.Add(tool.Key, toolRadio);
        }
    }

    private void ToolRadioButton_IsCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radio && radio.Name != null)
        {
            if (!radio.IsChecked.HasValue || !radio.IsChecked.Value)
            {
                return;
            }
            string name = radio.Name;

            _toolSelector.ToolSelected = _toolSelector.GetTool(name);
            _paintCanvas.ResetOverlay();
        }
    }

    private void Radio_Holding(object? sender, HoldingRoutedEventArgs e)
    {
        if (sender is ToolRadioButton radio)
        {
            OpenFlyoutFor(_toolSelector.GetTool(radio.Name), radio);
        }
    }

    private void Radio_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is ToolRadioButton radio)
        {
            var point = e.GetCurrentPoint(radio);

            if (!point.Properties.IsRightButtonPressed)
            {
                return;
            }

            OpenFlyoutFor(_toolSelector.GetTool(radio.Name), radio);
        }
    }

    private void SelectTool(string name)
    {
        if (_radios.TryGetValue(name, out ToolRadioButton? value))
        {
            value.IsChecked = true;
        }
    }

    private static void OpenFlyoutFor(BaseTool tool, ToolRadioButton radioButton)
    {
        var propertyItems = tool.GetToolProperties();

        if (propertyItems.Count == 0)
        {
            return;
        }

        ToolFlyout flyout = new(tool);
        FlyoutBase.SetAttachedFlyout(radioButton, flyout);
        FlyoutBase.ShowAttachedFlyout(radioButton);
    }
}
