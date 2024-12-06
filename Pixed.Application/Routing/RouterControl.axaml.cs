using Avalonia.Controls;
using AvaloniaInside.Shell;
using System.Threading.Tasks;

namespace Pixed.Application.Routing;

public partial class RouterControl : UserControl
{
    public RouterControl()
    {
        InitializeComponent();
        Router.Initialize(ShellViewMain.Navigator);
    }
}