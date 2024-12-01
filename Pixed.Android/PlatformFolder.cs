using Android.Content;
using Avalonia.Platform.Storage;
using Pixed.Common.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Android;
internal class PlatformFolder : IPlatformFolder
{
    public static Context Context { get; set; }
    public async Task<IStorageFolder> GetPixedFolder(IStorageProvider storageProvider)
    {
        var dir = Context.GetDir("Pixed", FileCreationMode.Private);
        Uri.TryCreate(dir.Path, UriKind.Absolute, out var path);
        return await storageProvider.TryGetFolderFromPathAsync(path);
    }
}
