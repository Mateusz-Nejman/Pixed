using Avalonia;
using Avalonia.Threading;
using Pixed.Application.IO;
using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Pages;

public partial class ProjectReceive : Modal, IDisposable
{
    private ProjectServer? _server;
    private ApplicationData? _applicationData;
    private PixedProjectMethods? _pixedProjectMethods;

    public string IpAddress
    {
        get { return GetValue(IpAddressProperty); }
        set { SetValue(IpAddressProperty, value); }
    }
    public string Status
    {
        get { return GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    public static readonly StyledProperty<string> StatusProperty = AvaloniaProperty.Register<ProjectReceive, string>("Status", "status");
    public static readonly StyledProperty<string> IpAddressProperty = AvaloniaProperty.Register<ProjectReceive, string>("IpAddress", "0.0.0.0");
    private bool disposedValue;

    public ProjectReceive()
    {
        InitializeComponent();
        Status = "Status: Listening";
        _applicationData = Provider?.Get<ApplicationData>();
        _pixedProjectMethods = Provider?.Get<PixedProjectMethods>();

        if (_applicationData != null && _pixedProjectMethods != null)
        {
            _server = new ProjectServer();
            Task.Run(TryReceiveProject);
        }
    }

    private async Task TryReceiveProject()
    {
        Dispatcher.UIThread.Invoke(() => IpAddress = NetUtils.GetIpAddress()?.ToString() ?? "0.0.0.0");
        if (_server == null || _applicationData == null || _pixedProjectMethods == null)
        {
            Dispatcher.UIThread.Invoke(() => Status = "Status: Error");
            return;
        }

        await _server.Listen(CheckProjectName, OnProjectReceived);
    }

    private async Task<bool> CheckProjectName(string name)
    {
        var result = await Router.Confirm("Receive project", $"Do you want to receive the project '{name}'?");

        if(result.HasValue)
        {
            if(result.Value == ButtonResult.Yes)
            {
                Dispatcher.UIThread.Invoke(() => Status = $"Status: Receiving project '{name}'");
            }

            return result.Value == ButtonResult.Yes;
        }

        return false;
    }

    private async Task OnProjectReceived(Stream stream)
    {
        if(_pixedProjectMethods == null)
        {
            return;
        }

        stream.Position = 0;
        await _pixedProjectMethods.Open(stream);
        await Close();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _server?.Dispose();
            }

            disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}