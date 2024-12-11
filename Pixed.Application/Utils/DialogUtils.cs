using Avalonia.Platform.Storage;
using Pixed.Common.Platform;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixed.Application.Utils;

internal class DialogUtils(IStorageProviderHandle storageProviderHandle)
{
    private readonly IStorageProviderHandle _storageProviderHandle = storageProviderHandle;

    public async Task<IReadOnlyList<IStorageFile>> OpenFileDialog(string filter, string filename, bool allowMultiple = false)
    {
        var storage = _storageProviderHandle.StorageProvider;
        return await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = allowMultiple,
            SuggestedFileName = filename,
            Title = "Open file",
            FileTypeFilter = GetFileFilter(filter)
        });
    }

    public async Task<IStorageFile?> SaveFileDialog(string filter, string filename)
    {
        return await _storageProviderHandle.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            FileTypeChoices = GetFileFilter(filter),
            ShowOverwritePrompt = true,
            SuggestedFileName = filename,
            Title = "Save file"
        });
    }

    private static List<FilePickerFileType> GetFileFilter(string filter)
    {
        var elems = filter.Split('|');

        if (elems.Length % 2 != 0)
        {
            return [];
        }

        List<FilePickerFileType> types = [];
        for (int a = 0; a < elems.Length; a += 2)
        {
            FilePickerFileType type = new(elems[a])
            {
                Patterns = elems[a + 1].Split(';')
            };
            types.Add(type);
        }

        return types;
    }
}
