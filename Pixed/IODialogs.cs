using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixed
{
    internal static class IODialogs
    {
        public static async Task<IReadOnlyList<IStorageFile>> OpenFileDialog(string filter, string filename)
        {
            var storage = MainWindow.Handle.StorageProvider;
            return await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                AllowMultiple = false,
                SuggestedFileName = filename,
                Title = "Open file",
                FileTypeFilter = GetFileFilter(filter)
            });
        }

        public static async Task<IStorageFile?> SaveFileDialog(string filter, string filename)
        {
            var storage = MainWindow.Handle.StorageProvider;
            return await storage.SaveFilePickerAsync(new FilePickerSaveOptions()
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
                FilePickerFileType type = new FilePickerFileType(elems[a]);
                type.Patterns = elems[a + 1].Split(';');
                types.Add(type);
            }

            return types;
        }
    }
}
