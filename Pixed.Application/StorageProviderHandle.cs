using Avalonia.Platform.Storage;
using Pixed.Application.Utils;
using Pixed.Common.Platform;
using System.Threading.Tasks;
namespace Pixed.Application;
internal class StorageProviderHandle : IStorageProviderHandle
{
    private IStorageProvider _storageProvider;
    public IStorageProvider StorageProvider => _storageProvider;
    public IPlatformFolder StorageFolder => PlatformUtils.platformFolder;

    public async Task<IStorageFolder> GetPixedFolder()
    {
        return await StorageFolder.GetPixedFolder(StorageProvider);
    }

    public async Task<IStorageFolder> GetPalettesFolder()
    {
        return await StorageFolder.GetPalettesFolder(StorageProvider);
    }
    public async Task<IStorageFolder> GetExtensionsFolder()
    {
        return await StorageFolder.GetExtensionsFolder(StorageProvider);
    }

    public void Initialize(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<string> GetPixedFolderAbsolute()
    {
        return await StorageFolder.GetPixedFolderAbsolute(StorageProvider);
    }

    public async Task<string> GetPalettesFolderAbsolute()
    {
        return await StorageFolder.GetPalettesFolderAbsolute(StorageProvider);
    }

    public async Task<string> GetExtensionsFolderAbsolute()
    {
        return await StorageFolder.GetExtensionsFolderAbsolute(StorageProvider);
    }
}
