using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Pixed.Windows;

public partial class OpenPNGWindow : Window
{
    public bool IsTileset { get; private set; }
    public int TileWidth => (int)tileWidth.Value;
    public int TileHeight => (int)tileHeight.Value;
    public OpenPNGWindow()
    {
        InitializeComponent();
        var radio = this.FindControl<RadioButton>("single");
        radio.IsChecked = true;
    }

    private void RadioButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if(sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            IsTileset = radioButton.Name == "tileset";
        }

        tileWidth.IsEnabled = IsTileset;
        tileHeight.IsEnabled = IsTileset;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}