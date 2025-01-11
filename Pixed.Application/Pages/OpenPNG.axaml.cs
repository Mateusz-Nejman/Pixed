using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Application.Models;

namespace Pixed.Application.Pages;

internal partial class OpenPNG : Modal
{
    public bool IsTileset { get; private set; }
    public OpenPNG()
    {
        InitializeComponent();
        var radio = this.FindControl<RadioButton>("single");
        radio.IsChecked = true;
    }

    private void RadioButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            IsTileset = radioButton.Name == "tileset";
        }

        tileWidth.IsEnabled = IsTileset;
        tileHeight.IsEnabled = IsTileset;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(new OpenPngResult((int)tileWidth.Value, (int)tileHeight.Value, IsTileset));
    }
}