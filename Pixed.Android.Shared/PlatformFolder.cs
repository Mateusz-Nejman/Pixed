﻿using Android.Content;
using Avalonia.Platform.Storage;
using Pixed.Application.IO;
using Pixed.Application.Platform;

namespace Pixed.Android;
internal class PlatformFolder : IPlatformFolder
{
    public static Context Context { get; set; }

    public IStorageContainerFile Convert(IStorageFile value)
    {
        return new PathStorageContainerFile(value.Path.AbsolutePath);
    }

    public async IAsyncEnumerable<IStorageContainerFile> GetFiles(FolderType folder)
    {
        string[] dirs = Directory.GetFiles(GetFolderPath(folder));

        foreach (string dir in dirs)
        {
            yield return new PathStorageContainerFile(dir);
        }
    }

    public Task<IStorageContainerFile> GetFile(string filename, FolderType folderType)
    {
        return Task.FromResult((IStorageContainerFile)new PathStorageContainerFile(Path.Combine(GetFolderPath(folderType), filename)));
    }

    private static string GetFolderPath(FolderType type)
    {
        return type switch
        {
            FolderType.Root => GetPixedFolder(),
            FolderType.Palettes => GetPalettesFolder(),
            FolderType.History => GetHistoryFolder(),
            _ => string.Empty,
        };
    }
    private static string GetPalettesFolder()
    {
        return GetFolderAbsolute("Palettes");
    }

    private static string GetPixedFolder()
    {
        return GetFolderAbsolute("Pixed");
    }

    private static string GetHistoryFolder()
    {
        return GetFolderAbsolute("History");
    }

    private static string GetFolderAbsolute(string folderName)
    {
        return Context.GetDir(folderName, FileCreationMode.Private).AbsolutePath;
    }
}
