using Avalonia;
using Avalonia.Controls;
using Pixed.Application.IO;

namespace Pixed.Application.Windows;

internal partial class YesNoCancelWindow : Window
{
    public string Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<YesNoCancelWindow, string>("Text", "Text");
    public YesNoCancelWindow()
    {
        InitializeComponent();
    }

    private void YesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CloseWithResult(ButtonResult.Yes);
    }

    private void NoButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CloseWithResult(ButtonResult.No);
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CloseWithResult(ButtonResult.Cancel);
    }

    private void CloseWithResult(ButtonResult result)
    {
        Close(result);
    }
}