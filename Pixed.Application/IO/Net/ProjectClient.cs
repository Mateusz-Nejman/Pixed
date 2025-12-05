using Pixed.Application.Utils;
using Pixed.Core.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

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

    public async Task<ProjectStatus> TrySendProject<T>(T address, PixedModel model)
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
            await TransferData.Write(_stream, model.FileName);
            Console.WriteLine($"ProjectClient {_client.DebugName}: Model name sent, waiting for response...");
            var transferData = await TransferData.Read(_stream);

            var response = transferData.ToString();

            Console.WriteLine($"ProjectClient {_client.DebugName}: Received response: " + response);
            if (response == "ACCEPT_MODEL")
            {
                MemoryStream modelStream = new();
                model.Serialize(modelStream);
                var modelBytes = modelStream.ToArray();
                await modelStream.DisposeAsync();
                await TransferData.Write(_stream, modelBytes);

                await _stream.DisposeAsync();
                _client.Dispose();
                return ProjectStatus.Accepted;
            }

            if (_stream != null)
            {
                await _stream.DisposeAsync();
            }
            _client?.Dispose();
            return ProjectStatus.Rejected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ProjectClient {_client?.DebugName ?? "Unknown"}: Exception occurred: " + ex.Message);
            if (_stream != null)
            {
                await _stream.DisposeAsync();
            }
            
            _client?.Dispose();
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