using Avalonia.Platform.Storage;
using Pixed.Application.Platform;
namespace Pixed.Application;
internal class StorageProviderHandle : IStorageProviderHandle
{
    private IStorageProvider _storageProvider;
    public IStorageProvider StorageProvider => _storageProvider;
    public void Initialize(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
}
