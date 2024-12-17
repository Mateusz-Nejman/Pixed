using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using System.Threading;
using System.Threading.Tasks;


namespace Pixed.Application.Windows;

internal partial class PromptWindow : PixedWindow
{
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set { SetValue(DefaultValueProperty, value); }
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<PromptWindow, string>("Text", string.Empty);
    public static readonly StyledProperty<string> DefaultValueProperty = AvaloniaProperty.Register<PromptWindow, string>("DefaultValue", string.Empty);
    public PromptWindow()
    {
        InitializeComponent();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if (args is PromptModel model)
        {
            Title = model.Title;
            Text = model.Text;
            DefaultValue = model.DefaultValue;
            textBox.Text = model.DefaultValue;
        }
        return Task.CompletedTask;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(textBox.Text);
    }
}
