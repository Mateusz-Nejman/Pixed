using Pixed.Application.Controls;
using Pixed.Application.ViewModels;

namespace Pixed.Application;

internal partial class MainView : ExtendedControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}