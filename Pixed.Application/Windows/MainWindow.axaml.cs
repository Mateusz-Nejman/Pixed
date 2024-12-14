using Avalonia.Controls;
using Pixed.Application.IO;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

internal partial class MainWindow : Window
{
    private readonly PixedProjectMethods _pixedProjectMethods;
    public MainWindow(PixedProjectMethods pixedProjectMethods)
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