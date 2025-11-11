using Newtonsoft.Json;
using Pixed.Application.IO;
using Pixed.Application.Menu;
using Pixed.Application.Platform;
using Pixed.Common.Menu;
using Pixed.Core;
using Pixed.Core.Models;
using Pixed.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.Services
{
    internal class RecentFilesService(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, IStorageProviderHandle storageProvider, IPlatformFolder platformFolder)
    {
        private readonly PixedProjectMethods _projectMethods = pixedProjectMethods;
        private readonly IStorageProviderHandle _storageProvider = storageProvider;
        private readonly IPlatformFolder _platformFolder = platformFolder;
        public List<string> RecentFiles { get; private set; } = [];

        public async Task Load()
        {
            var file = await _platformFolder.GetFile("recent.json", FolderType.Root);

            if (!file.Exists)
            {
                return;
            }

            Stream stream = await file.OpenRead();
            string json = stream.ReadAllText();
            stream.Dispose();
            RecentFiles = JsonConvert.DeserializeObject<List<string>>(json) ?? [];
        }

        public async Task AddRecent(string? file)
        {
            if(file == null)
            {
                return;
            }

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

        public async Task<List<IMenuItem>> BuildMenu()
        {
            if (RecentFiles.Count == 0)
            {
                return [];
            }

            List<IMenuItem> items = [];

            foreach (var file in RecentFiles)
            {
                var storageFile = await _storageProvider.StorageProvider.TryGetFileFromPathAsync(new Uri(file));
                if (storageFile != null)
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
            var file = await _platformFolder.GetFile("recent.json", FolderType.Root);
            var stream = await file.OpenWrite();
            stream.Write(JsonConvert.SerializeObject(RecentFiles));
            stream.Dispose();
        }

        private async Task OpenProject(string path)
        {
            RecentFiles.Remove(path);

            var file = await _storageProvider.StorageProvider.TryGetFileFromPathAsync(new Uri(path));
            if (file != null)
            {
                RecentFiles.Insert(0, path);
                await _projectMethods.Open(file);
            }

            await Save();
        }
    }
}
