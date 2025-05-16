using Avalonia.Platform.Storage;
using Pixed.Application.Platform;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
internal class DefaultPlatformFolder(IStorageProviderHandle handle) : IPlatformFolder
{
    private readonly IStorageProviderHandle _storageProviderHandle = handle;

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
        var filepath = Path.Combine(folder.Path.AbsolutePath, filename);

        IStorageFile? file = await _storageProviderHandle.StorageProvider.TryGetFileFromPathAsync(filepath);

        if (file == null)
        {
            return new PathStorageContainerFile(filepath);
        }

        return new DefaultStorageContainerFile(file);
    }

    private async Task<IStorageFolder> GetFolder(FolderType type)
    {
        return type switch
        {
            FolderType.Root => await GetPixedFolder(_storageProviderHandle.StorageProvider),
            FolderType.Palettes => await GetPalettesFolder(_storageProviderHandle.StorageProvider),
            FolderType.Extensions => await GetExtensionsFolder(_storageProviderHandle.StorageProvider),
            FolderType.History => await GetHistoryFolder(_storageProviderHandle.StorageProvider),
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

    private static async Task<IStorageFolder> GetHistoryFolder(IStorageProvider provider)
    {
        var pixedFolder = await GetPixedFolder(provider);
        return await pixedFolder.CreateFolderAsync("History");
    }
}
