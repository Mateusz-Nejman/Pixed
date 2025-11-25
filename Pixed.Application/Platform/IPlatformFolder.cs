using Avalonia.Platform.Storage;
using Pixed.Application.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixed.Application.Platform;
public interface IPlatformFolder
{
    public IStorageContainerFile? Convert(IStorageFile value);
    public Task<IStorageContainerFile?> GetFile(string filename, FolderType folderType);
    public IAsyncEnumerable<IStorageContainerFile?> GetFiles(FolderType folder);
}
