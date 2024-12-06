using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IPlatformFolder
{
    public Task<IStorageFolder> GetPixedFolder(IStorageProvider storageProvider);
    public Task<string> GetPixedFolderAbsolute(IStorageProvider storageProvider);
    public Task<IStorageFolder> GetPalettesFolder(IStorageProvider storageProvider);
    public Task<string> GetPalettesFolderAbsolute(IStorageProvider storageProvider);
    public Task<IStorageFolder> GetExtensionsFolder(IStorageProvider storageProvider);
    public Task<string> GetExtensionsFolderAbsolute(IStorageProvider storageProvider);
}
