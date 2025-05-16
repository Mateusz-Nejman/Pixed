using Avalonia.Platform.Storage;
using Pixed.Application.Utils;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
public interface IStorageContainerFile
{
    public string Name { get; }
    public string Path { get; }
    public string Extension { get; }
    public bool Exists { get; }
    public Task<StreamBase> OpenRead();
    public Task<StreamBase> OpenWrite();
    public Task Delete();
}

public class DefaultStorageContainerFile(IStorageFile file) : IStorageContainerFile
{
    private readonly IStorageFile _file = file;

    public string Name => _file.Name;

    public string Path => _file.Path.AbsolutePath;

    public string Extension => _file.GetExtension();
    public bool Exists => _file != null && File.Exists(Path);

    public async Task Delete()
    {
        await _file.DeleteAsync();
    }

    public async Task<StreamBase> OpenRead()
    {
        return StreamBase.CreateRead(await _file.OpenRead());
    }

    public async Task<StreamBase> OpenWrite()
    {
        return StreamBase.CreateWrite(await _file.OpenWrite());
    }
}

public class PathStorageContainerFile(string path) : IStorageContainerFile
{
    private readonly FileInfo _info = new(path);
    public string Name => _info.Name;

    public string Path => path;

    public string Extension => _info.Extension;
    public bool Exists => _info.Exists;

    public Task Delete()
    {
        _info.Delete();
        return Task.CompletedTask;
    }

    public Task<StreamBase> OpenRead()
    {
        return Task.FromResult(StreamBase.CreateRead(_info.OpenRead()));
    }

    public Task<StreamBase> OpenWrite()
    {
        return Task.FromResult(StreamBase.CreateWrite(_info.OpenWrite()));
    }
}