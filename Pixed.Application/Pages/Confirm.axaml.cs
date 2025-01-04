using Avalonia;
using Pixed.Application.IO;
using Pixed.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Pages;

internal partial class Confirm : Modal
{
    public string Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<Confirm, string>("Text", "Text");
    public Confirm()
    {
        InitializeComponent();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if (args is MessageModel model)
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