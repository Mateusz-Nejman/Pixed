using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Pixed.Application.IO.Net;
using Pixed.Application.Routing;
using Pixed.Application.ViewModels;
using Pixed.Core.Models;
using System;
using System.Threading.Tasks;

namespace Pixed.Application.Pages;

public partial class ProjectShare : Modal
{
    private readonly ProjectShareViewModel? _viewModel;
    public ProjectShare()
    {
        InitializeComponent();
        _viewModel = (ProjectShareViewModel?)DataContext;
        tcpRadio.IsChecked = true;
    }

    private void RadioButton_IsCheckedChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(sender is not RadioButton radio)
        {
            return;
        }

        if(radio.IsChecked == true)
        {
            _viewModel?.IsTcpEnabled = radio.Name == "tcpRadio";
        }
    }
}