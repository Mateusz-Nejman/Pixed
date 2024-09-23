using Avalonia.Controls;
using Newtonsoft.Json;
using Pixed.IO;
using Pixed.Models;
using System.Collections.Generic;
using System.IO;

namespace Pixed.Services
{
    internal class RecentFilesService
    {
        private readonly string _filePath;
        public List<string> RecentFiles { get; private set; }

        public RecentFilesService()
        {
            RecentFiles = [];
            _filePath = Path.Combine(Global.DataFolder, "recent.json");
        }

        public void Load()
        {
            if (File.Exists(_filePath))
            {
                RecentFiles = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_filePath));
            }
        }

        public void AddRecent(string file)
        {
            if (!RecentFiles.Contains(file))
            {
                RecentFiles.Insert(0, file);

                if (RecentFiles.Count == 10)
                {
                    RecentFiles.RemoveAt(RecentFiles.Count - 1);
                }

                Save();
            }
        }

        public NativeMenu? BuildMenu()
        {
            if (RecentFiles.Count == 0)
            {
                return null;
            }

            NativeMenu menu = [];

            foreach (var file in RecentFiles)
            {
                if (File.Exists(file))
                {
                    menu.Add(new NativeMenuItem(file)
                    {
                        Command = new ActionCommand<string>(OpenProject),
                        CommandParameter = file
                    });
                }
            }

            return menu;
        }

        private void Save()
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(RecentFiles));
        }

        private void OpenProject(string path)
        {
            RecentFiles.Remove(path);

            if (File.Exists(path))
            {
                RecentFiles.Insert(0, path);
                PixedProjectMethods.Open(path);
            }
            
            Save();
        }
    }
}
