using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

public class TcpTransferInterfaceServer : IProjectTransferInterfaceServer
{
    private readonly TcpListener _listener = new(IPAddress.Any, 80);

    public string DebugName => "Tcp";
    
    public void Start()
    {
        _listener.Start();
    }
    
    public void Stop()
    {
        _listener.Stop();
    }

    public async Task<IProjectTransferClient> Accept()
    {
        return new TcpTransferClient(await _listener.AcceptTcpClientAsync());
    }
}