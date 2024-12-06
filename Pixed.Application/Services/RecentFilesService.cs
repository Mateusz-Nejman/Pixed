using Newtonsoft.Json;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Common.Menu;
using Pixed.Common.Platform;
using Pixed.Core;
using Pixed.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Services
{
    internal class RecentFilesService(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, IStorageProviderHandle storageProvider)
    {
        private readonly PixedProjectMethods _projectMethods = pixedProjectMethods;
        private readonly IStorageProviderHandle _storageProvider = storageProvider;
        public List<string> RecentFiles { get; private set; } = [];

        public async Task Load()
        {
            var filePath = Path.Combine(await _storageProvider.GetPixedFolderAbsolute(), "recent.json");
            if (File.Exists(filePath))
            {
                RecentFiles = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(filePath));
            }
        }

        public async Task AddRecent(string file)
        {
            if (!RecentFiles.Contains(file))
            {
                RecentFiles.Insert(0, file);

                if (RecentFiles.Count == 10)
                {
                    RecentFiles.RemoveAt(RecentFiles.Count - 1);
                }

                await Save();
            }
        }

        public List<IMenuItem> BuildMenu()
        {
            if (RecentFiles.Count == 0)
            {
                return [];
            }

            List<IMenuItem> items = [];

            foreach (var file in RecentFiles)
            {
                if (File.Exists(file))
                {
                    items.Add(new MenuItem(file)
                    {
                        Command = new AsyncCommand<string>(OpenProject),
                        CommandParameter = file
                    });
                }
            }

            return items;
        }

        private async Task Save()
        {
            var filePath = Path.Combine(await _storageProvider.GetPixedFolderAbsolute(), "recent.json");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(RecentFiles));
        }

        private async Task OpenProject(string path)
        {
            RecentFiles.Remove(path);

            if (File.Exists(path))
            {
                RecentFiles.Insert(0, path);
                _projectMethods.Open(path);
            }

            await Save();
        }
    }
}
