using Pixed.Application.IO.Net;
using Pixed.Application.Utils;
using Pixed.Core.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO;

internal class ProjectClient(IProjectTransferInterfaceClient client) : IDisposable //Send project
{
    public enum ProjectStatus
    {
        Error,
        Accepted,
        Rejected
    }

    private readonly IProjectTransferInterfaceClient _client = client;

    private NetworkStream? _stream;

    private bool _disposedValue;

    public async Task<ProjectStatus> TrySendProject(object address, PixedModel model)
    {
        try
        {
            Console.WriteLine($"ProjectClient {_client.DebugName}: Connecting to " + address);
            await _client.Connect(address);

            if (_client.Connected)
            {
                Console.WriteLine($"ProjectClient {_client.DebugName}: Connected to " + address);
            }
            else
            {
                Console.WriteLine($"ProjectClient {_client.DebugName}: Cannot connect to " + address);
                _client.Dispose();
                return ProjectStatus.Error;
            }

            _stream = _client.GetStream();
            Console.WriteLine($"ProjectClient {_client.DebugName}: Sending model name " + model.FileName);
            _stream?.Write(model.FileName.ToNetBytes());
            Console.WriteLine($"ProjectClient {_client.DebugName}: Model name sent, waiting for response...");
            byte[] buffer = new byte[1024];
            int bytesRead;

            if (_stream != null)
            {
                bytesRead = await _stream.ReadAsync(buffer);
            }
            else
            {
                Console.WriteLine($"ProjectClient {_client.DebugName}: Stream is null");
                _client?.Dispose();
                return ProjectStatus.Error;
            }

            string response = buffer.ToNetMessage(bytesRead);

            Console.WriteLine($"ProjectClient {_client.DebugName}: Received response: " + response);
            if (response == "ACCEPT_MODEL")
            {
                MemoryStream modelStream = new();
                model.Serialize(modelStream);
                byte[] modelBytes = modelStream.ToArray();
                await modelStream.DisposeAsync();
                _stream.Write(modelBytes);
                await _stream.DisposeAsync();
                _client.Dispose();
                return ProjectStatus.Accepted;
            }
            else
            {
                await _stream.DisposeAsync();
                _client.Dispose();
                return ProjectStatus.Rejected;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ProjectClient {_client?.DebugName ?? "Unknown"}: Exception occurred: " + ex.Message);
            return ProjectStatus.Error;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client?.Dispose();
                _stream?.Dispose();
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