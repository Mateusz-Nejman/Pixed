using Avalonia.Controls;
using Pixed.Application.IO;
using System.IO;

namespace Pixed.Application.Windows;

internal partial class MainWindow : Window
{
    private readonly PixedProjectMethods _pixedProjectMethods;
    public MainWindow(PixedProjectMethods pixedProjectMethods)
    {
        InitializeComponent();
        _pixedProjectMethods = pixedProjectMethods;
    }

    public void OpenFromArgs(string[] args)
    {
        foreach (var arg in args)
        {
            if (File.Exists(arg))
            {
                _pixedProjectMethods.Open(arg);
            }
        }
    }
}