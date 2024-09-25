using Avalonia.Controls;
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
            Subjects.ToolChanged.OnNext(_toolSelector.ToolSelected);
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