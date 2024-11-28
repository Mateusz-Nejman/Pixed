using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace Pixed.Application.Controls;

public partial class RouterControl : UserControl
{
    public static INavigator Navigator { get; private set; }
    public RouterControl()
    {
        InitializeComponent();
        Navigator = ShellViewMain.Navigator;
    }
}