using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IStorageProviderHandle
{
    public void Initialize(IStorageProvider storageProvider);
    public IStorageProvider StorageProvider { get; }
    public IPlatformFolder StorageFolder { get; }
    public Task<IStorageFolder> GetPixedFolder();
    public Task<IStorageFolder> GetPalettesFolder();
    public Task<IStorageFolder> GetExtensionsFolder();
    public Task<string> GetPixedFolderAbsolute();
    public Task<string> GetPalettesFolderAbsolute();
    public Task<string> GetExtensionsFolderAbsolute();
}