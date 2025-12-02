using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

public interface IProjectTransferInterfaceServer
{
    public string DebugName { get; }
    public void Start();
    public void Stop();
    public Task<IProjectTransferClient> Accept();
}