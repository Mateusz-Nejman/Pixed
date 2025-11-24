using Avalonia.Interactivity;
using Pixed.Application.ViewModels;
using Pixed.Core.Models;
using System.Collections.Generic;

namespace Pixed.Application.Pages;

internal partial class ExportIcon : Modal
{
    private readonly ExportIconViewModel? _viewModel;
    public ExportIcon()
    {
        InitializeComponent();
        _viewModel = DataContext as ExportIconViewModel;
    }

    private List<Point> GetIconFormats()
    {
        if (_viewModel == null)
        {
            return [];
        }
        
        List<Point> formats = [];
        void AddIfTrue(bool isChecked, int resolution)
        {
            if (isChecked)
            {
                formats.Add(new Point(resolution));
            }
        }

        AddIfTrue(_viewModel.Check512, 512);
        AddIfTrue(_viewModel.Check256, 256);
        AddIfTrue(_viewModel.Check128, 128);
        AddIfTrue(_viewModel.Check64, 64);
        AddIfTrue(_viewModel.Check32, 32);
        AddIfTrue(_viewModel.Check16, 16);

        return formats;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(GetIconFormats());
    }
}