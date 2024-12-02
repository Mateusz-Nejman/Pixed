using Avalonia.Platform.Storage;
using Pixed.Common.Platform;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
public class DefaultPlatformFolder : IPlatformFolder
{
    public bool ExtensionsEnabled => true;

    public async Task<IStorageFolder> GetExtensionsFolder(IStorageProvider storageProvider)
    {
        var pixedFolder = await GetPixedFolder(storageProvider);
        return await pixedFolder.CreateFolderAsync("Extensions");
    }

    public async Task<string> GetExtensionsFolderAbsolute(IStorageProvider storageProvider)
    {
        return (await GetExtensionsFolder(storageProvider)).Path.AbsolutePath;
    }

    public async Task<IStorageFolder> GetPalettesFolder(IStorageProvider storageProvider)
    {
        var pixedFolder = await GetPixedFolder(storageProvider);
        return await pixedFolder.CreateFolderAsync("Palettes");
    }

    public async Task<string> GetPalettesFolderAbsolute(IStorageProvider storageProvider)
    {
        return (await GetPalettesFolder(storageProvider)).Path.AbsolutePath;
    }

    public async Task<IStorageFolder> GetPixedFolder(IStorageProvider provider)
    {
        var documentsFolder = await provider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        return await documentsFolder.CreateFolderAsync("Pixed");
    }

    public async Task<string> GetPixedFolderAbsolute(IStorageProvider storageProvider)
    {
        return (await GetPixedFolder(storageProvider)).Path.AbsolutePath;
    }
}
