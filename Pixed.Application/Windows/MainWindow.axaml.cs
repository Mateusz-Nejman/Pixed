using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Pixed.Application.Controls;
using Pixed.Application.ViewModels;
using Pixed.Common.Menu;

namespace Pixed.Application.Windows;

internal partial class MainWindow : Window
{
    public static Window Handle { get; private set; }
    public MainWindow()
    {
        Handle = this;
        InitializeComponent();
    }
}