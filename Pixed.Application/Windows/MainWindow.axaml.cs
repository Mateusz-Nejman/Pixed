using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Pixed.Application.Controls;
using Pixed.Application.ViewModels;
using Pixed.Common.Menu;

namespace Pixed.Application.Windows;

internal partial class MainWindow : PixedWindow<MainWindowViewModel>
{
    public static Window Handle { get; private set; }
    public MainWindow(IMenuItemRegistry menuItemRegistry) : base(menuItemRegistry)
    {
        Handle = this;
        InitializeComponent();
    }
}