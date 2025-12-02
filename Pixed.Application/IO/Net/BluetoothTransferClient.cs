using System;
using System.Net.Sockets;
using InTheHand.Net.Sockets;

namespace Pixed.Application.IO.Net;

public class BluetoothTransferClient(BluetoothClient client) : IProjectTransferClient
{
    private readonly BluetoothClient _client = client;
    private bool _disposedValue;

    public NetworkStream GetStream()
    {
        return _client.GetStream();
    }

    private void Dispose(bool disposing)
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