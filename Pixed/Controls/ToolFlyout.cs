using Avalonia.Controls;
using Pixed.Tools;

namespace Pixed.Controls;
internal class ToolFlyout(BaseTool tool) : Flyout
{
    private readonly BaseTool _tool = tool;

    protected override Control CreatePresenter()
    {
        base.CreatePresenter();
        return new FlyoutPresenter()
        {
            Content = BuildFlyoutContent()
        };
    }

    private StackPanel BuildFlyoutContent()
    {
        StackPanel stackPanel = new();

        var properties = _tool.GetCurrentProperties();

        foreach(var property in properties)
        {
            CheckBox checkBox = new()
            {
                Margin = new Avalonia.Thickness(5),
                Content = property.Name,
                IsChecked = property.Checked
            };
            checkBox.IsCheckedChanged += (s, e) =>
            {
                property.Checked = checkBox.IsChecked == true;
            };

            stackPanel.Children.Add(checkBox);
        }

        return stackPanel;
    }
}