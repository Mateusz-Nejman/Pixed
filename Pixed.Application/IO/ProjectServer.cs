using Pixed.Application.Utils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pixed.Application.IO;

internal class ProjectServer : IDisposable
{
    private bool _disposedValue;
    private bool _breakLoop = false;

    public async Task Listen(Func<string, Task<bool>> acceptFileFunc, Func<Stream, Task> projectReceived)
    {
        TcpListener listener = new(System.Net.IPAddress.Any, 13);
        Console.WriteLine("ProjectServer: Starting listener on port 13");
        listener.Start();
        while (!_breakLoop)
        {
            try
            {
                Console.WriteLine("ProjectServer: Waiting for incoming connection...");
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("ProjectServer: Client connected");
                var stream = client.GetStream();
                Console.WriteLine("ProjectServer: Reading file name...");
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer);
                var fileName = buffer.ToNetMessage(bytesRead);
                Console.WriteLine("ProjectServer: Received file name: " + fileName);
                var accept = await acceptFileFunc(fileName);

                if (accept)
                {
                    Console.WriteLine("ProjectServer: File accepted, sending ACCEPT_MODEL");
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
                Console.WriteLine("ProjectServer: Exception occurred: " + ex.Message);
            }
        }

        listener.Stop();
    }

    protected virtual void Dispose(bool disposing)
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