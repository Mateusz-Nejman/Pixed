using Avalonia;
using Avalonia.Threading;
using Pixed.Application.IO.Net;
using Pixed.Application.Routing;
using Pixed.Core.Models;
using System;
using System.Threading.Tasks;

namespace Pixed.Application.Pages;

public partial class ProjectSend : Modal, IDisposable
{
    private readonly ProjectClient? _client;
    private readonly ApplicationData? _applicationData;
    private bool _disposedValue;
    public string Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly StyledProperty<string> StatusProperty = AvaloniaProperty.Register<ProjectSend, string>("Status", "status");

    public ProjectSend()
    {
        InitializeComponent();
        Status = "Initializing...";
        _applicationData = Provider?.Get<ApplicationData>();

        if (_applicationData != null)
        {
            _client = new ProjectClient(new TcpTransferInterfaceClient());
            Task.Run(TrySendProject);
        }
    }

    private async Task TrySendProject()
    {
        if(_client == null || _applicationData == null)
        {
            ChangeStatus("Error");
            return;
        }

        var ipAddress = await Dispatcher.UIThread.Invoke(async() => await Router.Prompt("Ip Address", "Enter the server Ip Address:", "0.0.0.0"));

        if(!ipAddress.HasValue)
        {
            return;
        }

        await _client.TrySendProject(ipAddress.Value ?? "0.0.0.0", _applicationData.CurrentModel, ChangeStatus);
    }

    private void ChangeStatus(string status)
    {
        Dispatcher.UIThread.Invoke(() => Status = status);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client?.Dispose();
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