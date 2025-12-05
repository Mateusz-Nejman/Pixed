using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

internal class TcpTransferInterfaceClient : IProjectTransferInterfaceClient
{
    private bool _disposedValue;
    private readonly TcpClient _client;

    public string DebugName => "Bluetooth";
    public bool Connected => _client.Connected;

    public TcpTransferInterfaceClient()
    {
        _client = new TcpClient();
    }

    public async Task Connect(object address)
    {
        await _client.ConnectAsync(new IPEndPoint(IPAddress.Parse(address?.ToString() ?? "0.0.0.0"), TcpTransferInterfaceServer.TcpPort));
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