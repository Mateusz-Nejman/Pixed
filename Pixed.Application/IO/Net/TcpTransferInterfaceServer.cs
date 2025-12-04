using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

public class TcpTransferInterfaceServer : IProjectTransferInterfaceServer
{
    public const int TcpPort = 3333;
    private readonly TcpListener _listener = new(IPAddress.Any, TcpPort);

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