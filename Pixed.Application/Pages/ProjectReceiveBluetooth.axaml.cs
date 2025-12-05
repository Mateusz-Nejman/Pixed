using Avalonia;
using Avalonia.Threading;
using Pixed.Application.IO;
using Pixed.Application.Routing;
using Pixed.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Pixed.Application.IO.Net;

namespace Pixed.Application.Pages;

public partial class ProjectReceiveBluetooth : Modal, IDisposable
{
    private ProjectServer? _server;
    private ApplicationData? _applicationData;
    private PixedProjectMethods? _pixedProjectMethods;

    public string Status
    {
        get { return GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    public static readonly StyledProperty<string> StatusProperty = AvaloniaProperty.Register<ProjectReceiveBluetooth, string>("Status", "status");
    private bool _disposedValue;

    public ProjectReceiveBluetooth()
    {
        InitializeComponent();
        Status = "Status: Listening";
        _applicationData = Provider?.Get<ApplicationData>();
        _pixedProjectMethods = Provider?.Get<PixedProjectMethods>();

        if (_applicationData != null && _pixedProjectMethods != null)
        {
            _server = new ProjectServer(new BluetoothTransferInterfaceServer());
            Task.Run(TryReceiveProject);
        }
    }

    private async Task TryReceiveProject()
    {
        if (_server == null || _applicationData == null || _pixedProjectMethods == null)
        {
            Dispatcher.UIThread.Invoke(() => Status = "Status: Error");
            return;
        }

        await _server.Listen(CheckProjectName, OnProjectReceived);
    }

    private async Task<bool> CheckProjectName(string name)
    {
        var result = await Dispatcher.UIThread.Invoke(async() => await Router.Confirm("Receive project", $"Do you want to receive the project '{name}'?"));

        if (result.HasValue)
        {
            if (result.Value == ButtonResult.Yes)
            {
                Dispatcher.UIThread.Invoke(() => Status = $"Status: Receiving project '{name}'");
            }

            return result.Value == ButtonResult.Yes;
        }

        return false;
    }

    private async Task OnProjectReceived(Stream stream)
    {
        if (_pixedProjectMethods == null)
        {
            return;
        }

        stream.Position = 0;
        await _pixedProjectMethods.Open(stream);
        await Dispatcher.UIThread.InvokeAsync(Close);
    }

    protected virtual void Dispose(bool disposing)
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