using Avalonia.Metadata;
using Avalonia.OpenGL;
using Avalonia.Threading;
using InTheHand.Net.Sockets;
using Pixed.Application.Controls;
using Pixed.Application.IO;
using Pixed.Application.IO.Net;
using Pixed.Application.Pages;
using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class ProjectShareViewModel : ExtendedViewModel, IDisposable
{
    private ProjectClient? _currentClient;
    private ProjectServer? _currentServer;

    private readonly ApplicationData? _applicationData;
    private readonly PixedProjectMethods? _projectMethods;

    private string _status = string.Empty;
    private bool _isStatusVisible;

    private CancellationTokenSource _cancelationToken = new();
    private Task? _currentTask;

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public bool IsStatusVisible
    {
        get => _isStatusVisible;
        set
        {
            _isStatusVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsBluetoothPanelEnabled));
            OnPropertyChanged(nameof(IsTcpPanelEnabled));
            OnPropertyChanged(nameof(IsRadioEnabled));
        }
    }

    public bool IsRadioEnabled => !IsStatusVisible;

    #region TCP
    private bool _isTcpEnabled;
    private string _ipAddress = string.Empty;
    private string _destinationIpAddress = string.Empty;
    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            _ipAddress = value;
            OnPropertyChanged();
        }
    }

    public string DestinationIpAddress
    {
        get => _destinationIpAddress;
        set
        {
            _destinationIpAddress = value;
            OnPropertyChanged(nameof(DestinationIpAddress));
        }
    }

    public bool IsTcpEnabled
    {
        get => _isTcpEnabled;
        set
        {
            _isTcpEnabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTcpPanelEnabled));
            IsBluetoothEnabled = !value;
        }
    }

    public bool IsTcpPanelEnabled => IsTcpEnabled && !IsStatusVisible;

    #endregion
    #region Bluetooth
    private bool _isBluetoothEnabled;
    private BluetoothTransferInterfaceClient? _bluetoothInterface;

    private ObservableCollection<BluetoothDeviceInfo> _availableDevices = [];
    private BluetoothDeviceInfo? _selectedDevice;

    public ObservableCollection<BluetoothDeviceInfo> AvailableDevices
    {
        get => _availableDevices;
        set
        {
            _availableDevices = value;
            OnPropertyChanged();
        }
    }

    public BluetoothDeviceInfo? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            _selectedDevice = value;
            OnPropertyChanged();
        }
    }

    public bool IsBluetoothEnabled
    {
        get => _isBluetoothEnabled;
        set
        {
            _isBluetoothEnabled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsBluetoothPanelEnabled));
        }
    }

    public bool IsBluetoothPanelEnabled => IsBluetoothEnabled && !IsStatusVisible;
    #endregion

    public ICommand SendCommand { get; }
    public ICommand ReceiveCommand { get; }
    public ICommand CancelCommand { get; }

    public ProjectShareViewModel()
    {
        _applicationData = Provider?.Get<ApplicationData>();
        _projectMethods = Provider?.Get<PixedProjectMethods>();

        SendCommand = new AsyncCommand(SendAction);
        ReceiveCommand = new AsyncCommand(ReceiveAction);
        CancelCommand = new AsyncCommand(CancelAction);

        InitializeTcp();
        InitializeBluetooth();
    }

    private async Task SendAction()
    {
        _currentTask?.Dispose();
        _currentTask = Task.Run(SendTaskAction, _cancelationToken.Token);
    }

    private async Task ReceiveAction()
    {
        _currentTask?.Dispose();
        _currentTask = Task.Run(ReceiveTaskAction, _cancelationToken.Token);
    }

    private async Task CancelAction()
    {
        _cancelationToken.Cancel();
        ChangeStatus("Status: Cancelling...");
        Cleanup();
        IsStatusVisible = false;
        _cancelationToken.Dispose();
        _cancelationToken = new();
        IsStatusVisible = false;
    }

    private async Task SendTaskAction()
    {
        Dispatcher.UIThread.Invoke(() => IsStatusVisible = true);

        if (IsTcpEnabled && _applicationData?.CurrentModel != null)
        {
            _currentClient = new ProjectClient(new TcpTransferInterfaceClient());
            await _currentClient.TrySendProject(DestinationIpAddress, _applicationData.CurrentModel, ChangeStatus);
            Cleanup();
        }
        else if (IsBluetoothEnabled && _applicationData?.CurrentModel != null && SelectedDevice != null && _bluetoothInterface != null)
        {
            _bluetoothInterface?.Dispose();
            _bluetoothInterface = new BluetoothTransferInterfaceClient();
            _currentClient = new ProjectClient(_bluetoothInterface);
            await _currentClient.TrySendProject(SelectedDevice.DeviceAddress, _applicationData.CurrentModel, ChangeStatus);
            Cleanup();
        }

        Dispatcher.UIThread.Invoke(() => IsStatusVisible = false);
    }

    private async Task ReceiveTaskAction()
    {
        Dispatcher.UIThread.Invoke(() => IsStatusVisible = true);

        if (IsTcpEnabled && _applicationData?.CurrentModel != null)
        {
            _currentServer?.Dispose();
            _currentServer = new ProjectServer(new TcpTransferInterfaceServer());
            await _currentServer.Listen(CheckProjectName, OnProjectReceived, ChangeStatus, _cancelationToken.Token);

            if(_cancelationToken.Token.IsCancellationRequested)
            {
                _cancelationToken.Dispose();
                _cancelationToken = new();
                Dispatcher.UIThread.Invoke(() => IsStatusVisible = false);
            }

            Cleanup();
        }
        else if (IsBluetoothEnabled && _applicationData?.CurrentModel != null && SelectedDevice != null)
        {
            _currentServer?.Dispose();
            _currentServer = new ProjectServer(new BluetoothTransferInterfaceServer());
            await _currentServer.Listen(CheckProjectName, OnProjectReceived, ChangeStatus, _cancelationToken.Token);

            if (_cancelationToken.Token.IsCancellationRequested)
            {
                _cancelationToken.Dispose();
                _cancelationToken = new();
                Dispatcher.UIThread.Invoke(() => IsStatusVisible = false);
            }

            Cleanup();
        }
    }

    private void ChangeStatus(string status)
    {
        Dispatcher.UIThread.InvokeAsync(() => Status = status);
    }

    #region Server common
    private async Task<bool> CheckProjectName(string name)
    {
        var result = await Dispatcher.UIThread.Invoke(async () => await Router.Confirm("Receive project", $"Do you want to receive the project '{name}'?"));

        if (!result.HasValue) return false;
        if (result.Value == ButtonResult.Yes)
        {
            ChangeStatus($"Status: Receiving project '{name}'");
        }

        return result.Value == ButtonResult.Yes;

    }

    private async Task OnProjectReceived(Stream stream, string filename)
    {
        if (_projectMethods == null)
        {
            return;
        }

        stream.Position = 0;
        await _projectMethods.Open(stream, filename);
    }
    #endregion
    #region TCP
    private void InitializeTcp()
    {
        IpAddress = NetUtils.GetIpAddress()?.ToString() ?? "0.0.0.0";
        DestinationIpAddress = IpAddress;
    }
    #endregion
    #region Bluetooth
    private void InitializeBluetooth()
    {
        _bluetoothInterface = new BluetoothTransferInterfaceClient();
        AvailableDevices = new(_bluetoothInterface.GetDevices());
        if (AvailableDevices.Count > 0)
        {
            SelectedDevice = AvailableDevices[0];
        }
    }
    #endregion

    private void Cleanup()
    {
        if(IsBluetoothEnabled)
        {
            _currentServer?.Dispose();
            _currentServer = null;
        }
        _currentTask?.Dispose();
        _currentTask = null;
        _currentClient?.Dispose();
        _currentClient = null;
        _currentServer?.Dispose();
        _currentServer = null;
        _bluetoothInterface?.Dispose();
        _bluetoothInterface = null;
    }
    public void Dispose()
    {
        Cleanup();
    }
}