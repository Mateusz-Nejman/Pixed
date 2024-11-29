using Avalonia.Controls;
using AvaloniaInside.Shell;

namespace Pixed.Application;

public partial class RouterControl : UserControl
{
    public static INavigator Navigator { get; private set; }
    public RouterControl()
    {
        InitializeComponent();
        Navigator = ShellViewMain.Navigator;
    }
}