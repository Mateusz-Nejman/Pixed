using Avalonia.Controls;
using Avalonia.Interactivity;
using Pixed.Application.Models;
using Pixed.Core.Models;

namespace Pixed.Application.Pages;

internal partial class LoadLayer : Modal
{
    private bool _isSingle = true;
    public LoadLayer()
    {
        InitializeComponent();
        var radio = this.FindControl<RadioButton>("single");
        radio.IsChecked = true;
    }

    private void RadioButton_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            _isSingle = radioButton.Name == "single";
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(_isSingle);
    }
}