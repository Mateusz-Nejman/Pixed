using Android.Content;
using Avalonia.Platform.Storage;
using Pixed.Common.Platform;
using System;
using System.Threading.Tasks;

namespace Pixed.Android;
internal class PlatformFolder : IPlatformFolder
{
    public static Context Context { get; set; }

    public async Task<IStorageFolder> GetExtensionsFolder(IStorageProvider storageProvider)
    {
        return null;
    }

    public Task<string> GetExtensionsFolderAbsolute(IStorageProvider storageProvider)
    {
        return null;
    }

    public async Task<IStorageFolder> GetPalettesFolder(IStorageProvider storageProvider)
    {
        return await GetFolder(storageProvider, "Palettes");
    }

    public Task<string> GetPalettesFolderAbsolute(IStorageProvider storageProvider)
    {
        return Task.FromResult(GetFolderAbsolute("Palettes"));
    }

    public async Task<IStorageFolder> GetPixedFolder(IStorageProvider storageProvider)
    {
        return await GetFolder(storageProvider, "Pixed");
    }

    public Task<string> GetPixedFolderAbsolute(IStorageProvider storageProvider)
    {
        return Task.FromResult(GetFolderAbsolute("Pixed"));
    }

    private async Task<IStorageFolder> GetFolder(IStorageProvider storageProvider, string folderName)
    {
        GetFolderAbsolute(folderName);
        return await storageProvider.TryGetFolderFromPathAsync("/app_" + folderName);
    }

    private string GetFolderAbsolute(string folderName)
    {
        return Context.GetDir(folderName, FileCreationMode.Private).AbsolutePath;
    }
}
