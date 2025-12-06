using System;
using System.Net.Sockets;

namespace Pixed.Application.IO.Net;

public class TcpTransferClient(TcpClient client) : IProjectTransferClient
{
    private readonly TcpClient _client = client;
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