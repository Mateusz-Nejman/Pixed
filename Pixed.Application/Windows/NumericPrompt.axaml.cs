using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Pixed.Application.Windows;

public partial class NumericPrompt : Window
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

    public double Value { get; private set; } = 0;

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<NumericPrompt, string>("Text", string.Empty);
    public static readonly StyledProperty<double> DefaultValueProperty = AvaloniaProperty.Register<NumericPrompt, double>("DefaultValue", 0);
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<NumericPrompt, double>("Minimum", double.MinValue);
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<NumericPrompt, double>("Maximum", double.MaxValue);
    public NumericPrompt(double value = 0)
    {
        InitializeComponent();
        numeric.Value = (decimal)value;
        DefaultValue = value;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Value = (double)numeric.Value;
        Close(true);
    }
}