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
    public enum ProjectStatus
    {
        Error,
        Accepted,
        Rejected
    }

    private TcpClient? _openedClient;
    private NetworkStream? _openedStream;

    private bool _disposedValue;

    public async Task<ProjectStatus> TrySendProject(string ipAddress, PixedModel model)
    {
        try
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), 13);
            _openedClient = new TcpClient();
            Console.WriteLine("ProjectClient: Connecting to " + ipAddress);
            await _openedClient.ConnectAsync(endpoint);
            Console.WriteLine("ProjectClient: Connected to " + ipAddress);
            _openedStream = _openedClient.GetStream();
            Console.WriteLine("ProjectClient: Sending model name " + model.FileName);
            _openedStream?.Write(model.FileName.ToNetBytes());
            Console.WriteLine("ProjectClient: Model name sent, waiting for response...");
            byte[] buffer = new byte[1024];
            int bytesRead;

            if (_openedStream != null)
            {
                bytesRead = await _openedStream.ReadAsync(buffer);
            }
            else
            {
                Console.WriteLine("ProjectClient: Stream is null");
                _openedClient?.Dispose();
                return ProjectStatus.Error;
            }

            string response = buffer.ToNetMessage(bytesRead);

            Console.WriteLine("ProjectClient: Received response: " + response);
            if (response == "ACCEPT_MODEL")
            {
                MemoryStream modelStream = new();
                model.Serialize(modelStream);
                byte[] modelBytes = modelStream.ToArray();
                await modelStream.DisposeAsync();
                _openedStream.Write(modelBytes);
                await _openedStream.DisposeAsync();
                _openedClient.Dispose();
                return ProjectStatus.Accepted;
            }
            else
            {
                await _openedStream.DisposeAsync();
                _openedClient.Dispose();
                return ProjectStatus.Rejected;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ProjectClient: Exception occurred: " + ex.Message);
            return ProjectStatus.Error;
        }
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