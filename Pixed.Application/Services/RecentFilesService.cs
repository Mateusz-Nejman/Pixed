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
    internal class RecentFilesService(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, IStorageProviderHandle storageProvider)
    {
        private readonly PixedProjectMethods _projectMethods = pixedProjectMethods;
        private readonly IStorageProviderHandle _storageProvider = storageProvider;
        public List<string> RecentFiles { get; private set; } = [];

        public async Task Load()
        {
            var file = await _storageProvider.StorageFolder.GetFile("recent.json", FolderType.Root);

            Stream? stream = null;
            try
            {
                stream = await file.OpenRead();
                string json = stream.ReadAllText();
                stream?.Dispose();
                RecentFiles = JsonConvert.DeserializeObject<List<string>>(json) ?? [];
            }
            catch (Exception)
            {
                stream?.Dispose();
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
            var file = await _storageProvider.StorageFolder.GetFile("recent.json", FolderType.Root);
            var stream = await file.OpenWrite();
            stream.Write(JsonConvert.SerializeObject(RecentFiles));
            stream.Dispose();
        }

        private async Task OpenProject(string path)
        {
            RecentFiles.Remove(path);

            if (File.Exists(path))
            {
                RecentFiles.Insert(0, path);
                await _projectMethods.Open(path);
            }

            await Save();
        }
    }
}
