using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IStorageProviderHandle
{
    public void Initialize(IStorageProvider storageProvider);
    public IStorageProvider StorageProvider { get; }
    public IPlatformFolder StorageFolder { get; }
    public Task<IStorageFolder> GetPixedFolder();
}