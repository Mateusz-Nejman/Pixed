using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

internal class ProjectServer (IProjectTransferInterfaceServer transferInterface) : IDisposable
{
    private IProjectTransferInterfaceServer? _transferInterface = transferInterface;
    private bool _disposedValue;
    private bool _breakLoop;

    public async Task Listen(Func<string, Task<bool>> acceptFileFunc, Func<Stream, string, Task> projectReceived, Action<string> statusChangeAction, CancellationToken token)
    {
        if(_transferInterface == null)
        {
            return;
        }
        Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Starting listener");
        _transferInterface.Start();
        while (!_breakLoop)
        {
            NetworkStream? stream = null;
            IProjectTransferClient? client = null;
            try
            {
                if (_transferInterface == null)
                {
                    return;
                }
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Waiting for incoming connection...");
                statusChangeAction("Waiting for incoming connection...");
                client = await _transferInterface.Accept(token);
                token.ThrowIfCancellationRequested();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Client connected");
                statusChangeAction("Client connected");
                token.ThrowIfCancellationRequested();
                stream = client.GetStream();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Reading file name...");
                statusChangeAction("Reading file name...");
                token.ThrowIfCancellationRequested();
                var fileNameTransfer = await TransferData.Read(stream);
                token.ThrowIfCancellationRequested();
                var fileName = fileNameTransfer.ToString();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Received file name: " + fileName);
                statusChangeAction($"Received file name: {fileName}");
                token.ThrowIfCancellationRequested();
                var accept = await acceptFileFunc(fileName);
                token.ThrowIfCancellationRequested();

                if (accept)
                {
                    Console.WriteLine(
                        $"ProjectServer {_transferInterface.DebugName}: File accepted, sending ACCEPT_MODEL");
                    await TransferData.Write(stream, "ACCEPT_MODEL");
                    statusChangeAction("File accepted, receiving data");

                    token.ThrowIfCancellationRequested();
                    var fileTransfer = await TransferData.Read(stream);
                    token.ThrowIfCancellationRequested();
                    MemoryStream projectStream = new(fileTransfer.Data);
                    await projectReceived(projectStream, fileName);
                    statusChangeAction("File received");
                }

                await stream.DisposeAsync();
                client.Dispose();
            }
            catch (Exception ex)
            {
                if(stream != null)
                {
                    await stream.DisposeAsync();
                }
                client?.Dispose();
                Console.WriteLine($"ProjectServer {_transferInterface?.DebugName ?? "Unknown"}: Exception occurred: " + ex.Message);

                if(ex is OperationCanceledException)
                {
                    break;
                }
            }
        }

        _transferInterface?.Stop();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _transferInterface?.Dispose();
                _transferInterface = null;
                _breakLoop = true;
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