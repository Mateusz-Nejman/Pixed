using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

internal interface IProjectTransferInterfaceClient : IDisposable
{
    public string DebugName { get; }
    public bool Connected { get; }
    public Task Connect(object address);
    public NetworkStream GetStream();
}