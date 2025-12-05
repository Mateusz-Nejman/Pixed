using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using Pixed.Application.IO;
using Pixed.Application.IO.Net;
using Pixed.Application.Pages;
using Pixed.Application.Routing;
using Pixed.Core;
using Pixed.Core.Models;

namespace Pixed.Application.ViewModels;

public class ProjectReceiveViewModel : PropertyChangedBase, IDisposable
{
    private readonly ProjectServer? _server;
    private readonly ApplicationData? _applicationData;
    private readonly PixedProjectMethods? _pixedProjectMethods;
    private readonly Modal _modal;

    private string _status = string.Empty;
    private string _ipAddress = string.Empty;
    private bool _isIpAddressVisible = true;
    private bool _disposedValue;

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            _ipAddress = value;
            OnPropertyChanged();
        }
    }

    public bool IsIpAddressVisible
    {
        get => _isIpAddressVisible;
        set
        {
            _isIpAddressVisible = value;
            OnPropertyChanged();
        }
    }

    public ProjectReceiveViewModel(Modal modal, IProjectTransferInterfaceServer transferInterface)
    {
        _modal = modal;
        Status = "Initializing...";
        _applicationData = App.ServiceProvider?.Get<ApplicationData>();
        _pixedProjectMethods = App.ServiceProvider?.Get<PixedProjectMethods>();

        _server = new ProjectServer(transferInterface);
    }
    
    public async Task TryReceiveProject()
    {
        if (_server == null || _applicationData == null || _pixedProjectMethods == null)
        {
            ChangeStatus("Error");
            return;
        }

        await _server.Listen(CheckProjectName, OnProjectReceived, ChangeStatus);
    }

    private void ChangeStatus(string status)
    {
        Dispatcher.UIThread.InvokeAsync(() => Status = status);
    }

    private async Task<bool> CheckProjectName(string name)
    {
        var result = await Dispatcher.UIThread.Invoke(async() => await Router.Confirm("Receive project", $"Do you want to receive the project '{name}'?"));

        if (!result.HasValue) return false;
        if (result.Value == ButtonResult.Yes)
        {
            ChangeStatus($"Status: Receiving project '{name}'");
        }

        return result.Value == ButtonResult.Yes;

    }

    private async Task OnProjectReceived(Stream stream, string filename)
    {
        if (_pixedProjectMethods == null)
        {
            return;
        }

        stream.Position = 0;
        await _pixedProjectMethods.Open(stream, filename);
        await Dispatcher.UIThread.InvokeAsync(_modal.Close);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _server?.Dispose();
            }

            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}