using Avalonia.Threading;
using InTheHand.Net.Sockets;
using Pixed.Application.IO.Net;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class ProjectSendBluetoothViewModel : PropertyChangedBase, IDisposable
{
    private readonly ProjectClient _client;
    private readonly BluetoothTransferInterfaceClient _interface;
    private readonly ApplicationData? _applicationData;

    private ObservableCollection<BluetoothDeviceInfo> _availableDevices = [];
    private BluetoothDeviceInfo? _selectedDevice;
    private string _status = string.Empty;
    private bool _statusVisible;
    private bool _isButtonEnabled;
    private bool _disposedValue;

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

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public bool StatusVisible
    {
        get => _statusVisible;
        set
        {
            _statusVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set
        {
            _isButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    public ICommand SendCommand { get; }

    public ProjectSendBluetoothViewModel()
    {
        Status = "Initializing...";
        _applicationData = App.ServiceProvider?.Get<ApplicationData>();
        _interface = new BluetoothTransferInterfaceClient();
        _client = new ProjectClient(_interface);
        SendCommand = new AsyncCommand(SendAction);

        Initialize();
    }

    private void Initialize()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AvailableDevices = new(_interface.GetDevices());
            if(AvailableDevices.Count > 0)
            {
                SelectedDevice = AvailableDevices[0];
                IsButtonEnabled = true;
            }
        });
    }

    private async Task SendAction()
    {
        StatusVisible = true;
        if(_applicationData == null || SelectedDevice == null)
        {
            ChangeAction("Error");
            return;
        }

        await _client.TrySendProject(SelectedDevice.DeviceAddress, _applicationData.CurrentModel, ChangeAction);
    }

    private void ChangeAction(string status)
    {
        Dispatcher.UIThread.Invoke(() => Status = status);
    }

    private void Dispose(bool disposing)
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