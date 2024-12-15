using Avalonia.Platform.Storage;
using Pixed.Application.IO;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Android;
internal class PlatformStorageContainerFile(string file) : IStorageContainerFile
{
    private readonly FileInfo _info = new(file);

    public string Name => _info.Name;

    public string Path => _info.FullName;
    public string Extension => _info.Extension;

    public PlatformStorageContainerFile(IStorageFile file) : this(file.Path.AbsolutePath)
    {
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
