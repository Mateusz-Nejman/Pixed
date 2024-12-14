using Avalonia;
using Pixed.Application.IO;
using Pixed.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class ConfirmWindow : PixedWindow
{
    public string Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ConfirmWindow, string>("Text", "Text");
    public ConfirmWindow()
    {
        InitializeComponent();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if (args is ConfirmModel model)
        {
            Title = model.Title;
            Text = model.Text;
        }
        return Task.CompletedTask;
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