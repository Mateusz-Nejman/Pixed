using System;
using System.IO;
using System.Threading.Tasks;
using Pixed.Application.Utils;
using Pixed.Core.Utils;

namespace Pixed.Application.IO.Net;

public readonly struct TransferData(byte[] data)
{
    public byte[] Data { get; } = data;
    public int Length => Data.Length;

    public async Task Write(Stream? stream)
    {
        if (stream == null)
        {
            return;
        }

        Console.WriteLine("Writing TransferData of length: " + Length);
        await stream.WriteIntAsync(Length);
        await stream.WriteAsync(Data);
    }

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

    public static async Task<TransferData> Read(Stream stream)
    {
        var length = await stream.ReadIntAsync();
        Console.WriteLine("Read TransferData with length: " + length);
        int bytesRead;
        int bytesLeft = length;
        byte[] buffer = new byte[512];
        MemoryStream memoryStream = new();
        // Keep reading until the stream ends
        while ((bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, bytesLeft))) > 0)
        {
            bytesLeft -= bytesRead;
            await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));

            if(bytesRead == length)
            {
                break;
            }
        }

        if (memoryStream.Length != length)
        {
            throw new Exception("Expect to read " + length + " bytes, but only read " + memoryStream.Length + " bytes.");
        }

        return new TransferData(memoryStream.ToArray());
    }
}