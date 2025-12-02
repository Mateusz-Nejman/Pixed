using System;
using System.Net.Sockets;

namespace Pixed.Application.IO.Net;

public interface IProjectTransferClient : IDisposable
{
    public NetworkStream GetStream();
}