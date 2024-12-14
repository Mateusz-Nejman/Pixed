using Avalonia;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class NumericPromptWindow : PixedWindow
{
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set { SetValue(DefaultValueProperty, value); }
    }

    public double Minimum
    {
        get => GetValue(DefaultValueProperty);
        set { SetValue(DefaultValueProperty, value); }
    }

    public double Maximum
    {
        get => GetValue(DefaultValueProperty);
        set { SetValue(DefaultValueProperty, value); }
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<NumericPromptWindow, string>("Text", string.Empty);
    public static readonly StyledProperty<double> DefaultValueProperty = AvaloniaProperty.Register<NumericPromptWindow, double>("DefaultValue", 0);
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<NumericPromptWindow, double>("Minimum", double.MinValue);
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<NumericPromptWindow, double>("Maximum", double.MaxValue);
    public NumericPromptWindow()
    {
        InitializeComponent();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if (args is NumericPromptModel model)
        {
            Title = model.Title;
            Text = model.Text;
            DefaultValue = model.DefaultValue;
            numeric.Value = (decimal)model.DefaultValue;
        }
        return Task.CompletedTask;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close((double)numeric.Value);
    }
}