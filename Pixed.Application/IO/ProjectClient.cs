using Pixed.Application.Utils;
using Pixed.Core.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO;

internal class ProjectClient : IDisposable //Send project
{
    private TcpClient? _openedClient;
    private NetworkStream? _openedStream;

    private bool _disposedValue;

    public async Task<bool> TrySendProject(PixedModel model)
    {
        var servers = await NetUtils.FindLocalServers();

        foreach (var server in servers)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(server), 13);
            _openedClient = new TcpClient();
            await _openedClient.ConnectAsync(endpoint);
            _openedStream = _openedClient.GetStream();
            _openedStream?.Write(model.FileName.ToNetBytes());
            byte[] buffer = new byte[1024];
            int bytesRead;

            if(_openedStream != null)
            {
                bytesRead = await _openedStream.ReadAsync(buffer);
            }
            else
            {
                _openedClient?.Dispose();
                break;
            }

            string response = buffer.ToNetMessage(bytesRead);

            if(response == "ACCEPT_MODEL")
            {
                MemoryStream modelStream = new();
                model.Serialize(modelStream);
                byte[] modelBytes = modelStream.ToArray();
                await modelStream.DisposeAsync();
                _openedStream.Write(modelBytes);
                await _openedStream.DisposeAsync();
                _openedClient.Dispose();
                return true;
            }
            else
            {
                await _openedStream.DisposeAsync();
                _openedClient.Dispose();
            }
        }

        return false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _openedClient?.Dispose();
                _openedStream?.Dispose();
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