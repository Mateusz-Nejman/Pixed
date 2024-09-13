using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Tools;
using Pixed.ViewModels;

namespace Pixed.Controls.MainWindowSections;

internal partial class ToolsSection : UserControl
{
    public PaintCanvasViewModel? PaintCanvas { get; set; }
    public ToolsSection()
    {
        InitializeComponent();
        Global.ToolSelector = new ToolSelector(SelectTool);
    }

    private void ToolRadioButton_IsCheckedChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radio && radio.Name != null)
        {
            string name = radio.Name;

            Global.ToolSelected = Global.ToolSelector.GetTool(name);
            PaintCanvas?.ResetOverlay();
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