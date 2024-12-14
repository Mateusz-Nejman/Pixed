using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using System.Threading;
using System.Threading.Tasks;


namespace Pixed.Application.Windows;

internal partial class MessageWindow : PixedWindow
{
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<PromptWindow, string>("Text", string.Empty);
    public MessageWindow()
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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
