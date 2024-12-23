using Pixed.Application.Controls;
using Pixed.Application.ViewModels;

namespace Pixed.Application;

internal partial class MainView : PixedUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}