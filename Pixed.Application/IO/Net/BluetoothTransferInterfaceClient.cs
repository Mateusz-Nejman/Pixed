using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

internal class BluetoothTransferInterfaceClient : IProjectTransferInterfaceClient
{
    private bool _disposedValue;
    private readonly BluetoothClient _client;

    public string DebugName => "Bluetooth";

    public bool Connected => _client.Connected;

    public BluetoothTransferInterfaceClient()
    {
        _client = new BluetoothClient();
    }

    public IEnumerable<BluetoothDeviceInfo> GetDevices()
    {
        return _client.PairedDevices;
    }

    public Task Connect<T>(T address)
    {
        if (address is BluetoothAddress btAddress)
        {
            _client.Connect(new BluetoothEndPoint(btAddress, BluetoothService.SerialPort));
        }
        else
        {
            _client.Connect(new BluetoothEndPoint(BluetoothAddress.Parse(address?.ToString()), BluetoothService.SerialPort));
        }
        return Task.CompletedTask;
    }

    public NetworkStream GetStream()
    {
        return _client.GetStream();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client.Dispose();
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