using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Pixed.Tools;
using Pixed.ViewModels;

namespace Pixed.Controls.MainWindowSections;

internal partial class ToolsSection : PixedUserControl<ToolsSectionViewModel>
{
    private readonly ToolSelector _toolSelector;
    private readonly PaintCanvasViewModel _paintCanvas;
    public ToolsSection() : base()
    {
        InitializeComponent();
        _toolSelector = Provider.Get<ToolSelector>();
        _toolSelector.SelectToolAction = SelectTool;
        _paintCanvas = Provider.Get<PaintCanvasViewModel>();
        InitializeFlyouts();
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

    private void SelectTool(string name)
    {
        var obj = this.FindControl<RadioButton>(name);

        if (obj != null)
        {
            obj.IsChecked = true;
        }
    }

    private void InitializeFlyouts()
    {
        var tools = _toolSelector.GetTools();

        foreach (var toolPair in tools)
        {
            var obj = this.FindControl<ToolRadioButton>(toolPair.Key);
            obj.PointerPressed += Radio_PointerPressed;
            obj.Holding += Radio_Holding;
        }
    }

    private void Radio_Holding(object? sender, HoldingRoutedEventArgs e)
    {
        if (sender is ToolRadioButton radio)
        {
            OpenFlyout(radio);
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

            OpenFlyout(radio);
        }
    }

    private void OpenFlyout(ToolRadioButton radio)
    {
        ViewModel.OpenFlyoutFor(_toolSelector.GetTool(radio.Name), radio);
    }
}