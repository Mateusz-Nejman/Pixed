using Pixed.Models;
using Pixed.Windows;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.IO;
internal static class PixedProjectMethods
{
    public async static Task Save(PixedModel model, bool saveAs)
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
            string name;
            if (string.IsNullOrEmpty(model.FileName))
            {
                name = "project.pixed";
            }
            else
            {
                FileInfo info = new(model.FileName ?? string.Empty);
                name = info.Name.Replace(info.Extension, string.Empty);
            }
            var file = await IODialogs.SaveFileDialog("Pixed project (*.pixed)|*.pixed", name);

            if (file == null)
            {
                return;
            }

            model.FilePath = file.Path.AbsolutePath;
            fileStream = await file.OpenWriteAsync();
        }

        if (fileStream != null)
        {
            PixedProjectSerializer serializer = new();
            serializer.Serialize(fileStream, model, true);
            Global.RecentFilesService.AddRecent(model.FilePath);
        }
    }

    public async static Task Open()
    {
        var files = await IODialogs.OpenFileDialog("All supported (*.pixed;*.png)|*.pixed;*.png|Pixed project (*.pixed)|*.pixed|PNG images (*.png)|*.png", "Open file", true);

        foreach (var item in files)
        {
            var stream = await item.OpenReadAsync();

            IPixedProjectSerializer serializer;
            if (item.Name.EndsWith(".pixed"))
            {
                serializer = new PixedProjectSerializer();

                Global.RecentFilesService.AddRecent(item.Path.AbsolutePath);
            }
            else
            {
                serializer = new PngProjectSerializer();
            }

            PixedModel model = serializer.Deserialize(stream);
            stream?.Dispose();
            model.FileName = item.Name.Replace(".png", ".pixed");
            model.AddHistory();

            if (item.Name.EndsWith(".pixed"))
            {
                model.FilePath = item.Path.AbsolutePath;
            }

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

    public static void Open(string path)
    {
        FileInfo info = new(path);
        PixedProjectSerializer serializer = new();
        Stream stream = File.OpenRead(path);
        PixedModel model = serializer.Deserialize(stream);
        stream?.Dispose();

        model.FileName = info.Name;
        model.FilePath = path;
        model.AddHistory();

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

    public async static Task ExportToPng(PixedModel model)
    {
        string name;
        if (string.IsNullOrEmpty(model.FileName))
        {
            name = "pixed.png";
        }
        else
        {
            FileInfo info = new(model.FileName ?? string.Empty);
            name = info.Name.Replace(info.Extension, string.Empty);
        }
        var file = await IODialogs.SaveFileDialog("PNG image (*.png)|*.png", name);

        if (file == null)
        {
            return;
        }

        PngProjectSerializer serializer = new();

        int columnsCount = 1;
        if (model.Frames.Count > 1)
        {
            ExportPNGWindow window = new();
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
