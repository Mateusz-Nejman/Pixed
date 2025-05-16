using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Svg.Skia;
using Pixed.Application.Controls;
using Pixed.Application.Extensions;
using Pixed.Common.Tools;
using System;
using System.Collections.Generic;
using IPlatformSettings = Pixed.Application.Platform.IPlatformSettings;

namespace Pixed.Application.ViewModels;
internal class ToolsSectionViewModel(ToolsManager toolSelector, PaintControlViewModel paintCanvas) : ExtendedViewModel
{
    private readonly ToolsManager _toolSelector = toolSelector;
    private readonly PaintControlViewModel _paintCanvas = paintCanvas;
    private readonly Dictionary<string, ToolRadioButton> _radios = [];

    public void InitializeTools(StackPanel stackPanel)
    {
        _toolSelector.SelectToolAction = SelectTool;
        _radios.Clear();
        var tools = _toolSelector.GetTools();

        if (IPlatformSettings.Instance.ExtensionsEnabled)
        {
            var extensionTools = ExtensionsLoader.GetTools(App.ServiceProvider);

            foreach (var extensionTool in extensionTools)
            {
                tools.Add(extensionTool.Key, extensionTool.Value);
            }
        }

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

            if (Avalonia.Application.Current.TryFindResource(hasCustomProperties ? "ToolRadioCustom" : "ToolRadio", out var res) && res is ControlTheme theme)
            {
                toolRadio.Theme = theme;
            }

            if (AssetLoader.Exists(new Uri(tool.Value.ImagePath)))
            {
                var stream = AssetLoader.Open(new Uri(tool.Value.ImagePath));
                IImage? image = null;

                if (tool.Value.ImagePath.EndsWith(".svg"))
                {
                    image = new SvgImage() { Source = SvgSource.LoadFromStream(stream) };
                }
                else
                {
                    image = new Bitmap(stream);
                }
                toolRadio.Source = image;
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

            _toolSelector.SelectedTool = _toolSelector.GetTool(name);
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
