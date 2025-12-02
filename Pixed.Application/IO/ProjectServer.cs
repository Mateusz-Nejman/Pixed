using Pixed.Application.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using Pixed.Application.IO.Net;

namespace Pixed.Application.IO;

internal class ProjectServer : IDisposable
{
    private readonly IProjectTransferInterfaceServer _transferInterface;
    private bool _disposedValue;
    private bool _breakLoop;

    public ProjectServer(IProjectTransferInterfaceServer transferInterface)
    {
        _transferInterface = transferInterface;
    }

    public async Task Listen(Func<string, Task<bool>> acceptFileFunc, Func<Stream, Task> projectReceived)
    {
        Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Starting listener on port 13");
        _transferInterface.Start();
        while (!_breakLoop)
        {
            try
            {
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Waiting for incoming connection...");
                var client = await _transferInterface.Accept();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Client connected");
                var stream = client.GetStream();
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Reading file name...");
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer);
                var fileName = buffer.ToNetMessage(bytesRead);
                Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: Received file name: " + fileName);
                var accept = await acceptFileFunc(fileName);

                if (accept)
                {
                    Console.WriteLine($"ProjectServer {_transferInterface.DebugName}: File accepted, sending ACCEPT_MODEL");
                    await stream.WriteAsync("ACCEPT_MODEL".ToNetBytes());

                    MemoryStream projectStream = new();
                    while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
                    {
                        projectStream.Write(buffer, 0, bytesRead);
                    }
                    projectStream.Position = 0;
                    await projectReceived(projectStream);
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