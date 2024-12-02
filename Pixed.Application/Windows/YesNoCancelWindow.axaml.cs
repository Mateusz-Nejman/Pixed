using Avalonia;
using Pixed.Application.IO;

namespace Pixed.Application.Windows;

internal abstract partial class YesNoCancelWindow : PixedWindow
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
        Close(ButtonResult.Yes);
    }

    private void NoButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(ButtonResult.No);
    }

    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(ButtonResult.Cancel);
    }
}