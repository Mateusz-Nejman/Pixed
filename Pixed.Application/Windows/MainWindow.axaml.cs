using Avalonia.Controls;
using Pixed.Application.Controls;
using Pixed.Application.IO;
using Pixed.Application.ViewModels;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class MainWindow : PixedWindow<MainViewModel>
{
    private readonly PixedProjectMethods _pixedProjectMethods;
    public MainWindow(PixedProjectMethods pixedProjectMethods) : base()
    {
        InitializeComponent();
        _pixedProjectMethods = pixedProjectMethods;
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
}