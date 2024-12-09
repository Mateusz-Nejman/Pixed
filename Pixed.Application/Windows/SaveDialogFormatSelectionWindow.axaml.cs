using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Pixed.Application.ViewModels;
using Pixed.Application.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.Windows;

public partial class SaveDialogFormatSelectionWindow : PixedWindow
{
    private readonly SaveDialogFormatSelectionViewModel _viewModel;
    public SaveDialogFormatSelectionWindow()
    {
        InitializeComponent();
        _viewModel = (SaveDialogFormatSelectionViewModel)DataContext;
        _viewModel.CloseAction = () => Close(_viewModel.Formats[_viewModel.SelectionIndex]);
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        if(args is List<FilePickerFileType> files)
        {
            _viewModel.Formats = files;
        }

        return Task.CompletedTask;
    }
}