using Avalonia.Platform.Storage;
using Pixed.Application.Platform;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
public class DefaultPlatformFolder : IPlatformFolder
{
    private IStorageProvider _storageProvider;
    public IStorageContainerFile Convert(IStorageFile value)
    {
        return new DefaultStorageContainerFile(value);
    }

    public async IAsyncEnumerable<IStorageContainerFile> GetFiles(FolderType type)
    {
        var folder = await GetFolder(type);
        await foreach (var item in folder.GetItemsAsync())
        {
            if (item is IStorageFile file)
            {
                yield return new DefaultStorageContainerFile(file);
            }
        }
    }

    public async Task<IStorageContainerFile> GetFile(string filename, FolderType folderType)
    {
        var folder = await GetFolder(folderType);
        return new DefaultStorageContainerFile(await _storageProvider.TryGetFileFromPathAsync(Path.Combine(folder.Path.AbsolutePath, filename)));
    }

    public void Initialize(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    private async Task<IStorageFolder> GetFolder(FolderType type)
    {
        return type switch
        {
            FolderType.Root => await GetPixedFolder(_storageProvider),
            FolderType.Palettes => await GetPalettesFolder(_storageProvider),
            FolderType.Extensions => await GetExtensionsFolder(_storageProvider),
            _ => throw new System.NotImplementedException()
        };
    }

    private static async Task<IStorageFolder> GetExtensionsFolder(IStorageProvider storageProvider)
    {
        var pixedFolder = await GetPixedFolder(storageProvider);
        return await pixedFolder.CreateFolderAsync("Extensions");
    }

    private static async Task<IStorageFolder> GetPalettesFolder(IStorageProvider storageProvider)
    {
        var pixedFolder = await GetPixedFolder(storageProvider);
        return await pixedFolder.CreateFolderAsync("Palettes");
    }

    private static async Task<IStorageFolder> GetPixedFolder(IStorageProvider provider)
    {
        var documentsFolder = await provider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        return await documentsFolder.CreateFolderAsync("Pixed");
    }
}
