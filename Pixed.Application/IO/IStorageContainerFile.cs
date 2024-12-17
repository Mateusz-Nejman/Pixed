using Avalonia.Platform.Storage;
using Pixed.Application.Utils;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
public interface IStorageContainerFile
{
    public string Name { get; }
    public string Path { get; }
    public string Extension { get; }
    public Task<StreamBase> OpenRead();
    public Task<StreamBase> OpenWrite();
}

public class DefaultStorageContainerFile(IStorageFile file) : IStorageContainerFile
{
    private readonly IStorageFile _file = file;

    public string Name => _file.Name;

    public string Path => _file.Path.AbsolutePath;

    public string Extension => _file.GetExtension();

    public async Task<StreamBase> OpenRead()
    {
        return StreamBase.CreateRead(await _file.OpenRead());
    }

    public async Task<StreamBase> OpenWrite()
    {
        return StreamBase.CreateWrite(await _file.OpenWrite());
    }
}