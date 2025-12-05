using System;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.IO.Net;

internal class ProjectServer (IProjectTransferInterfaceServer transferInterface) : IDisposable
{
    private readonly IProjectTransferInterfaceServer _transferInterface = transferInterface;
    private bool _disposedValue;
    private bool _breakLoop;

    public async Task Listen(Func<string, Task<bool>> acceptFileFunc, Func<Stream, string, Task> projectReceived, Action<string> statusChangeAction)
    {
        Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Starting listener");
        _transferInterface.Start();
        while (!_breakLoop)
        {
            try
            {
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Waiting for incoming connection...");
                statusChangeAction("Waiting for incoming connection...");
                var client = await _transferInterface.Accept();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Client connected");
                statusChangeAction("Client connected");
                var stream = client.GetStream();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Reading file name...");
                statusChangeAction("Reading file name...");
                var fileNameTransfer = await TransferData.Read(stream);
                var fileName = fileNameTransfer.ToString();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Received file name: " + fileName);
                statusChangeAction($"Received file name: {fileName}");
                var accept = await acceptFileFunc(fileName);

                if (accept)
                {
                    Console.WriteLine(
                        $"ProjectServer {_transferInterface.DebugName}: File accepted, sending ACCEPT_MODEL");
                    await TransferData.Write(stream, "ACCEPT_MODEL");
                    statusChangeAction("File accepted, receiving data");

                    var fileTransfer = await TransferData.Read(stream);
                    MemoryStream projectStream = new(fileTransfer.Data);
                    await projectReceived(projectStream, fileName);
                    statusChangeAction("File received");
                }

                await stream.DisposeAsync();
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Exception occurred: " + ex.Message);
            }
        }

        _transferInterface.Stop();
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
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