using Pixed.Models;
using Pixed.Services;
using Pixed.Utils;
using Pixed.Windows;
using System.IO;
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
            string extension = (new FileInfo(item.Name)).Extension;

            IPixedProjectSerializer? serializer = GetSerializer(extension);

            if (serializer == null || !serializer.CanDeserialize)
            {
                continue;
            }

            if (serializer is PixedProjectSerializer)
            {
                recentFilesService.AddRecent(item.Path.AbsolutePath);
            }

            if(serializer is PngProjectSerializer pngSerializer)
            {
                OpenPNGWindow window = new OpenPNGWindow();
                var success = await window.ShowDialog<bool>(MainWindow.Handle);

                if(success)
                {
                    if(window.IsTileset)
                    {
                        pngSerializer.TileWidth = window.TileWidth;
                        pngSerializer.TileHeight = window.TileHeight;
                    }
                }
                else
                {
                    continue;
                }
            }

            PixedModel model = serializer.Deserialize(stream, _applicationData);
            stream?.Dispose();
            model.FileName = item.Name.Replace(".png", ".pixed");
            model.AddHistory();

            if (serializer is PixedProjectSerializer)
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
        FileInfo info = new(path);
        IPixedProjectSerializer? serializer = GetSerializer(info.Extension);

        if (serializer == null || !serializer.CanDeserialize)
        {
            return;
        }
        Stream stream = File.OpenRead(path);

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

    public async Task ExportToIco(PixedModel model)
    {
        FileInfo info = new(model.FileName);
        string name = info.Name;

        if (!string.IsNullOrEmpty(info.Extension))
        {
            name = info.Name.Replace(info.Extension, string.Empty);
        }
        var file = await DialogUtils.SaveFileDialog("Icon (*.ico)|*.ico", name);

        if (file == null)
        {
            return;
        }

        IconProjectSerializer serializer = new();

        if (model.Frames.Count == 1)
        {
            ExportIconWindow window = new ExportIconWindow();
            var success = await window.ShowDialog<bool>(MainWindow.Handle);

            if (!success)
            {
                return;
            }

            serializer.IconFormats = window.IconFormats;
        }
        var stream = await file.OpenWriteAsync();
        serializer.Serialize(stream, model, true);
    }

    private IPixedProjectSerializer? GetSerializer(string format)
    {
        return format switch
        {
            ".pixed" => new PixedProjectSerializer(),
            ".png" => new PngProjectSerializer(),
            ".piskel" => new PiskelProjectSerializer(),
            ".ico" => new IconProjectSerializer(),
            _ => null,
        };
    }
}
