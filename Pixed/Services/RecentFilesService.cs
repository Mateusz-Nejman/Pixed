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
            RecentFiles.Insert(0, path);
            Save();
            FileInfo info = new FileInfo(path);
            PixedProjectSerializer serializer = new();
            Stream stream = File.OpenRead(path);
            PixedModel model = serializer.Deserialize(stream);
            stream?.Dispose();

            model.FileName = info.Name;
            model.FilePath = path;

            if (Global.CurrentModel.IsEmpty)
            {
                Global.Models[Global.CurrentModelIndex] = model;
            }
            else
            {
                Global.Models.Add(model);
            }
            Subjects.ProjectAdded.OnNext(model);
        }
    }
}
