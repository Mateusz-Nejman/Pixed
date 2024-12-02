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

    public void Initialize(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
}
