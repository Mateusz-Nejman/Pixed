using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace Pixed.Common.Utils;
public static class StorageProviderUtils
{
    public async static Task<IStorageFolder?> GetPixedFolder(this IStorageProvider provider)
    {
        var documentsFolder = await provider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        return await documentsFolder.CreateFolderAsync("Pixed");
    }
}
