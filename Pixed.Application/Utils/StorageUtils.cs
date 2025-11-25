using Avalonia.Platform.Storage;
using Pixed.Application.IO;
using Pixed.Core.Utils;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Utils;
internal static class StorageUtils
{
    public static async Task<Stream?> OpenWrite(this IStorageFile file)
    {
        return StreamBase.CreateWrite(await file.OpenWriteAsync());
    }

    public static async Task<Stream?> OpenRead(this IStorageFile file)
    {
        return StreamBase.CreateRead(await file.OpenReadAsync());
    }

    public static async Task CopyTo(this IStorageFile input, IStorageFile output)
    {
        var inputStream = await input.OpenRead();

        if (inputStream == null)
        {
            return;
        }
        
        var bytes = inputStream.ReadAllBytes();
        var outputStream = await output.OpenWrite();

        if (outputStream == null)
        {
            await inputStream.DisposeAsync();
            return;
        }
        outputStream.Write(bytes);
        await inputStream.DisposeAsync();
        await outputStream.DisposeAsync();
    }

    public static async Task CopyTo(this IStorageFile input, IStorageContainerFile output)
    {
        var inputStream = await input.OpenRead();

        if (inputStream == null)
        {
            return;
        }
        
        var bytes = inputStream.ReadAllBytes();
        var outputStream = await output.OpenWrite();

        if (outputStream == null)
        {
            await inputStream.DisposeAsync();
            return;
        }
        
        outputStream.Write(bytes);
        await inputStream.DisposeAsync();
        await outputStream.DisposeAsync();
    }

    public static async Task CopyTo(this IStorageContainerFile input, IStorageContainerFile output)
    {
        var inputStream = await input.OpenRead();

        if (inputStream == null)
        {
            return;
        }
        
        var bytes = inputStream.ReadAllBytes();
        var outputStream = await output.OpenWrite();

        if (outputStream == null)
        {
            await inputStream.DisposeAsync();
            return;
        }
        
        outputStream.Write(bytes);
        await inputStream.DisposeAsync();
        await outputStream.DisposeAsync();
    }

    public static string GetExtension(this IStorageFile file)
    {
        string name = file.Name;
        string[] parts = name.Split('.');

        if (parts.Length >= 2)
        {
            return "." + parts[^1];
        }

        return string.Empty;
    }
}
