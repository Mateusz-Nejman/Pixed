using Avalonia.Platform.Storage;
using Pixed.Application.IO;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Utils;
internal static class StorageUtils
{
    public static async Task<Stream> OpenWrite(this IStorageFile file)
    {
        return StreamBase.Create(await file.OpenWriteAsync());
    }

    public static async Task<Stream> OpenRead(this IStorageFile file)
    {
        return StreamBase.Create(await file.OpenReadAsync());
    }
}
