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
        var buffer = new byte[length];
        
        var bytesRead = await stream.ReadAsync(buffer);

        if (bytesRead != length)
        {
            //TODO exception
        }
        
        return new TransferData(buffer);
    }
}