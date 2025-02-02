using Avalonia.Platform.Storage;

namespace Pixed.Application.Platform;
public interface IStorageProviderHandle
{
    public void Initialize(IStorageProvider storageProvider);
    public IStorageProvider StorageProvider { get; }
}