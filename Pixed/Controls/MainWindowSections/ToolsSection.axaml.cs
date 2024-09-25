using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
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
        _toolSelector = ServiceProvider.GetService<ToolSelector>();
        _toolSelector.Action = SelectTool; //TODO find better way
        _paintCanvas = ServiceProvider.GetService<PaintCanvasViewModel>();
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

            Global.ToolSelected = _toolSelector.GetTool(name);
            Subjects.ToolChanged.OnNext(Global.ToolSelected);
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
}