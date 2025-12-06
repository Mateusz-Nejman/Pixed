using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Pixed.Application.Utils;
using Pixed.Core.Utils;

namespace Pixed.Application.IO.Net;

public readonly struct TransferData(byte[] data)
{
    public byte[] Data { get; } = data;
    private int Length => Data.Length;
    
    public override string ToString()
    {
        return Data.ToNetMessage();
    }

    public static async Task Write(Stream? stream, string message)
    {
        await Write(stream, message.ToNetBytes());
    }

    public static async Task Write(Stream? stream, byte[] buffer)
    {
        var data = new TransferData(buffer);
        await data.Write(stream);
    }

    [SuppressMessage("Performance", "CA1835:Prefer the \'Memory\'-based overloads for \'ReadAsync\' and \'WriteAsync\'")]
    public static async Task<TransferData> Read(Stream stream)
    {
        var length = await stream.ReadIntAsync();
        Console.WriteLine("Read TransferData with length: " + length);
        int bytesRead;
        var bytesLeft = length;
        var buffer = new byte[512];
        MemoryStream memoryStream = new();
        // Keep reading until the stream ends
        while ((bytesRead = await stream.ReadAsync(buffer, 0, Math.Min(buffer.Length, bytesLeft))) > 0) //Fixes https://stackoverflow.com/questions/40559549/android-bluetooth-java-io-ioexception-bt-socket-closed-read-return-1
        {
            bytesLeft -= bytesRead;
            await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));

            if(bytesRead == length)
            {
                break;
            }
        }

        return memoryStream.Length != length ? throw new Exception("Expect to read " + length + " bytes, but only read " + memoryStream.Length + " bytes.") : new TransferData(memoryStream.ToArray());
    }
    
    private async Task Write(Stream? stream)
    {
        if (stream == null)
        {
            return;
        }

        Console.WriteLine("Writing TransferData of length: " + Length);
        await stream.WriteIntAsync(Length);
        await stream.WriteAsync(Data);
    }
}