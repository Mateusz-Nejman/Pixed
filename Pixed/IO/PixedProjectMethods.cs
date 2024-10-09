using Pixed.Models;
using Pixed.Services;
using Pixed.Utils;
using Pixed.Windows;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.IO;
internal class PixedProjectMethods(ApplicationData applicationData)
{
    private readonly ApplicationData _applicationData = applicationData;

    public async Task Save(PixedModel model, bool saveAs, RecentFilesService recentFilesService)
    {
        Stream fileStream = null;
        if (model.FilePath == null)
        {
            saveAs = true;
        }
        else
        {
            fileStream = File.OpenWrite(model.FilePath);
        }

        if (saveAs)
        {
            FileInfo info = new(model.FileName);
            string name = info.Name;

            if (!string.IsNullOrEmpty(info.Extension))
            {
                name = info.Name.Replace(info.Extension, string.Empty);
            }
            var file = await DialogUtils.SaveFileDialog("Pixed project (*.pixed)|*.pixed", name);

            if (file == null)
            {
                return;
            }

            model.FilePath = file.Path.AbsolutePath;
            model.FileName = file.Name;
            fileStream = await file.OpenWriteAsync();
        }

        if (fileStream != null)
        {
            PixedProjectSerializer serializer = new();
            serializer.Serialize(fileStream, model, true);
            recentFilesService.AddRecent(model.FilePath);
            model.UnsavedChanges = false;
            Subjects.ProjectChanged.OnNext(model);
        }
    }

    public async Task Open(RecentFilesService recentFilesService)
    {
        var files = await DialogUtils.OpenFileDialog("All supported (*.pixed;*.piskel;*.png)|*.pixed;*.piskel;*.png|Pixed project (*.pixed)|*.pixed|Piskel project (*.piskel)|*.piskel|PNG images (*.png)|*.png", "Open file", true);

        foreach (var item in files)
        {
            var stream = await item.OpenReadAsync();

            IPixedProjectSerializer serializer;
            if (item.Name.EndsWith(".pixed"))
            {
                serializer = new PixedProjectSerializer();

                recentFilesService.AddRecent(item.Path.AbsolutePath);
            }
            else if (item.Name.EndsWith(".piskel"))
            {
                serializer = new PiskelProjectSerializer();
            }
            else
            {
                serializer = new PngProjectSerializer();
            }

            PixedModel model = serializer.Deserialize(stream, _applicationData);
            stream?.Dispose();
            model.FileName = item.Name.Replace(".png", ".pixed");
            model.AddHistory();

            if (item.Name.EndsWith(".pixed"))
            {
                model.FilePath = item.Path.AbsolutePath;
                model.UnsavedChanges = false;
            }

            if (_applicationData.CurrentModel.IsEmpty)
            {
                _applicationData.Models[_applicationData.CurrentModelIndex] = model;
            }
            else
            {
                _applicationData.Models.Add(model);
            }

            Subjects.ProjectAdded.OnNext(model);
        }
    }

    public void Open(string path)
    {
        string[] formats = [".pixed", ".piskel", ".png"];
        FileInfo info = new(path);

        if (!formats.Contains(info.Extension))
        {
            return;
        }

        IPixedProjectSerializer serializer;
        Stream stream = File.OpenRead(path);

        if (path.EndsWith(".pixed"))
        {
            serializer = new PixedProjectSerializer();
        }
        else if (path.EndsWith(".piskel"))
        {
            serializer = new PiskelProjectSerializer();
        }
        else
        {
            serializer = new PngProjectSerializer();
        }

        PixedModel model = serializer.Deserialize(stream, _applicationData);
        stream.Dispose();
        model.FileName = info.Name.Replace(".png", ".pixed");
        model.AddHistory();

        if (info.Name.EndsWith(".pixed"))
        {
            model.FilePath = info.FullName;
            model.UnsavedChanges = false;
        }

        if (_applicationData.CurrentModel.IsEmpty)
        {
            _applicationData.Models[_applicationData.CurrentModelIndex] = model;
        }
        else
        {
            _applicationData.Models.Add(model);
        }

        Subjects.ProjectAdded.OnNext(model);
    }

    public async Task ExportToPng(PixedModel model)
    {
        FileInfo info = new(model.FileName);
        string name = info.Name;

        if (!string.IsNullOrEmpty(info.Extension))
        {
            name = info.Name.Replace(info.Extension, string.Empty);
        }
        var file = await DialogUtils.SaveFileDialog("PNG image (*.png)|*.png", name);

        if (file == null)
        {
            return;
        }

        PngProjectSerializer serializer = new();

        int columnsCount = 1;
        if (model.Frames.Count > 1)
        {
            ExportPNGWindow window = new(_applicationData);
            bool success = await window.ShowDialog<bool>(MainWindow.Handle);

            if (!success)
            {
                return;
            }

            columnsCount = window.ColumnsCount;
        }

        serializer.ColumnsCount = columnsCount;
        var stream = await file.OpenWriteAsync();
        serializer.Serialize(stream, model, true);
    }
}
