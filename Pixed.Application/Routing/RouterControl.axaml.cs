using Avalonia.Controls;

namespace Pixed.Application.Routing;

public partial class RouterControl : UserControl
{
    public RouterControl()
    {
        InitializeComponent();
        Router.Initialize(ShellViewMain.Navigator);
    }
}