using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace Pixed.Common.Platform;
public interface IPlatformFolder
{
    public Task<IStorageFolder> GetPixedFolder(IStorageProvider storageProvider);
}
