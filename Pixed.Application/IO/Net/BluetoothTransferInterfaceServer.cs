using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace Pixed.Application.IO.Net;

public class BluetoothTransferInterfaceServer : IProjectTransferInterfaceServer
{
    private readonly BluetoothListener _listener;
    private bool _disposedValue;

    public string DebugName => "Bluetooth";

    public BluetoothTransferInterfaceServer()
    {
        _listener = new BluetoothListener(BluetoothService.SerialPort);
    }
    
    public void Start()
    {
        _listener.Start();
    }

    public void Stop()
    {
        _listener?.Stop();
    }

    public async Task<IProjectTransferClient> Accept(CancellationToken token)
    {
        return new BluetoothTransferClient(await _listener.AcceptBluetoothClientAsync());
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _listener.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}