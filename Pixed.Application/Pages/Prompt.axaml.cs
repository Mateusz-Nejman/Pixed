using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using System.Threading;
using System.Threading.Tasks;


namespace Pixed.Application.Pages;

internal partial class Prompt : Modal
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

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<Prompt, string>("Text", string.Empty);
    public static readonly StyledProperty<string> DefaultValueProperty = AvaloniaProperty.Register<Prompt, string>("DefaultValue", string.Empty);
    public Prompt()
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
