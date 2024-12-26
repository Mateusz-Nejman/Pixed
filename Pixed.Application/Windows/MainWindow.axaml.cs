using Avalonia.Controls;
using Pixed.Application.Controls;
using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Application.ViewModels;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class MainWindow : PixedWindow<MainViewModel>
{
    private readonly PixedProjectMethods _pixedProjectMethods;
    private readonly IPlatformSettings _lifecycle;
    public MainWindow(PixedProjectMethods pixedProjectMethods, IPlatformSettings lifecycle) : base()
    {
        InitializeComponent();
        _pixedProjectMethods = pixedProjectMethods;
        _lifecycle = lifecycle;
    }

    public async Task OpenFromArgs(string[] args)
    {
        foreach (var arg in args)
        {
            if (File.Exists(arg))
            {
                await _pixedProjectMethods.Open(arg);
            }
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        e.Cancel = true;
        var canQuit = await MainPage.Close();

        if(canQuit)
        {
            _lifecycle.Close();
        }
    }
}