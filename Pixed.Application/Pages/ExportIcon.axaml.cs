using Avalonia.Interactivity;
using Pixed.Application.ViewModels;
using Pixed.Core.Models;
using System.Collections.Generic;

namespace Pixed.Application.Pages;

internal partial class ExportIcon : Modal
{
    private readonly ExportIconViewModel _viewModel;
    public ExportIcon()
    {
        InitializeComponent();
        _viewModel = DataContext as ExportIconViewModel;
    }

    private List<Point> GetIconFormats()
    {
        List<Point> formats = [];
        void addIfTrue(bool isChecked, int resolution)
        {
            if (isChecked)
            {
                formats.Add(new Point(resolution));
            }
        }

        addIfTrue(_viewModel.Check512, 512);
        addIfTrue(_viewModel.Check256, 256);
        addIfTrue(_viewModel.Check128, 128);
        addIfTrue(_viewModel.Check64, 64);
        addIfTrue(_viewModel.Check32, 32);
        addIfTrue(_viewModel.Check16, 16);

        return formats;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close(GetIconFormats());
    }
}