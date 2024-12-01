using Avalonia.Platform.Storage;
using Pixed.Common.Platform;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
public class DefaultPlatformFolder : IPlatformFolder
{
    public async Task<IStorageFolder> GetPixedFolder(IStorageProvider provider)
    {
        var documentsFolder = await provider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        return await documentsFolder.CreateFolderAsync("Pixed");
    }
}
