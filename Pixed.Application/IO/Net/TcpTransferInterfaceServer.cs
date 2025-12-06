using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

public class TcpTransferInterfaceServer : IProjectTransferInterfaceServer
{
    public const int TcpPort = 3333;
    private readonly TcpListener _listener = new(IPAddress.Any, TcpPort);
    private bool _disposedValue;

    public string DebugName => "Tcp";
    
    public void Start()
    {
        _listener.Start();
    }
    
    public void Stop()
    {
        _listener.Stop();
    }

    public async Task<IProjectTransferClient> Accept(CancellationToken token)
    {
        return new TcpTransferClient(await _listener.AcceptTcpClientAsync(token));
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