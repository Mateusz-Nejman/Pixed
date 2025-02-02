using Avalonia.Platform.Storage;
using Pixed.Application.Models;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Services;
using Pixed.Application.Utils;
using Pixed.Common;
using Pixed.Core.Models;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
internal class PixedProjectMethods(ApplicationData applicationData, DialogUtils dialogUtils, IStorageProviderHandle storageProvider)
{
    private readonly ApplicationData _applicationData = applicationData;
    private readonly DialogUtils _dialogUtils = dialogUtils;
    private readonly IStorageProviderHandle _storageProvider = storageProvider;

    public async Task Save(PixedModel model, bool saveAs, RecentFilesService recentFilesService)
    {
        Stream fileStream = null;
        if (model.FilePath == null)
        {
            saveAs = true;
        }
        else
        {
            try
            {
                var file = await _storageProvider.StorageProvider.TryGetFileFromPathAsync(new Uri(model.FilePath));

                if (file != null)
                {
                    fileStream = await file.OpenWrite();
                }
                else
                {
                    await Router.Message("Error", "Can't open " + model.FilePath);
                    return;
                }
            }
            catch (Exception) //On android FilePath can contains one-use FilePath
            {
                saveAs = true;
            }
        }

        if (saveAs)
        {
            FileInfo info = new(model.FileName);
            string name = info.Name;

            if (!string.IsNullOrEmpty(info.Extension))
            {
                name = info.Name.Replace(info.Extension, string.Empty);
            }

            if (IPlatformSettings.Instance.ExtensionsOnSave)
            {
                name += ".pixed";
            }
            var file = await _dialogUtils.SaveFileDialog("Pixed project (*.pixed)|*.pixed", name);

            if (file == null)
            {
                return;
            }

            model.FilePath = file.Path.AbsolutePath;
            model.FileName = file.Name;
            fileStream = await file.OpenWrite();
        }

        if (fileStream != null && fileStream.CanWrite)
        {
            PixedProjectSerializer serializer = new();
            serializer.Serialize(fileStream, model, true);
            await recentFilesService.AddRecent(model.FilePath);
            model.UnsavedChanges = false;
            Subjects.ProjectChanged.OnNext(model);
        }
    }

    public async Task Open(RecentFilesService recentFilesService)
    {
        var files = await _dialogUtils.OpenFileDialog("All supported (*.pixed;*.piskel;*.png;*.svg)|*.pixed;*.piskel;*.png|Pixed project (*.pixed)|*.pixed|Piskel project (*.piskel)|*.piskel|PNG images (*.png)|*.png|SVG images (*.svg)|*.svg", "Open file", true);

        foreach (var item in files)
        {
            var stream = await item.OpenRead();
            string extension = (new FileInfo(item.Name)).Extension;

            IPixedProjectSerializer? serializer = GetSerializer(extension);

            if (serializer == null || !serializer.CanDeserialize)
            {
                continue;
            }

            if (serializer is PixedProjectSerializer)
            {
                await recentFilesService.AddRecent(item.Path.AbsolutePath);
            }

            if (serializer is PngProjectSerializer pngSerializer)
            {
                var result = await Router.Navigate<OpenPngResult>("/openPng");

                if (result.HasValue)
                {
                    if (result.Value.IsTileset)
                    {
                        pngSerializer.TileWidth = result.Value.TileWidth;
                        pngSerializer.TileHeight = result.Value.TileHeight;
                    }
                }
                else
                {
                    continue;
                }
            }

            if (serializer is SvgProjectSerializer svgSerializer)
            {
                SKSvg svg = SKSvg.CreateFromStream(stream);
                stream.Dispose();
                stream = await item.OpenRead();
                var result = await Router.Navigate<OpenSvgResult>("/openSvg", svg);

                if (result.HasValue)
                {
                    svgSerializer.Width = result.Value.Width;
                    svgSerializer.Height = result.Value.Height;
                }
                else
                {
                    continue;
                }
            }

            PixedModel model;
            try
            {
                model = serializer.Deserialize(stream, _applicationData);
            }
            catch (Exception _)
            {
                await Router.Message("Opening error", "Invalid format");
                continue;
            }
            stream?.Dispose();
            model.FileName = item.Name.Replace(item.GetExtension(), ".pixed");
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

    public async Task Open(string path)
    {
        var file = await _storageProvider.StorageProvider.TryGetFileFromPathAsync(new Uri(path));

        if (file == null)
        {
            return;
        }

        await Open(file);
    }

    public async Task Open(IStorageFile? file)
    {
        if (file == null)
        {
            return;
        }

        IPixedProjectSerializer? serializer = GetSerializer(file.GetExtension());

        if (serializer == null || !serializer.CanDeserialize)
        {
            return;
        }

        Stream stream = await file.OpenReadAsync();

        PixedModel model;
        try
        {
            model = serializer.Deserialize(stream, _applicationData);
        }
        catch (Exception _)
        {
            await Router.Message("Opening error", "Invalid format");
            return;
        }
        stream.Dispose();
        model.FileName = file.Name.Replace(".png", ".pixed");
        model.AddHistory();

        if (file.Name.EndsWith(".pixed"))
        {
            model.FilePath = file.Path.AbsolutePath;
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

        if (IPlatformSettings.Instance.ExtensionsOnSave)
        {
            name += ".png";
        }
        var file = await _dialogUtils.SaveFileDialog("PNG image (*.png)|*.png", name);

        if (file == null)
        {
            return;
        }

        PngProjectSerializer serializer = new();

        int columnsCount = 1;
        if (model.Frames.Count > 1)
        {
            var result = await Router.Navigate<int>("/exportPng");

            if (result.HasValue)
            {
                columnsCount = result.Value;
            }
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
        if (IPlatformSettings.Instance.ExtensionsOnSave)
        {
            name += ".ico";
        }
        var file = await _dialogUtils.SaveFileDialog("Icon (*.ico)|*.ico", name);

        if (file == null)
        {
            return;
        }

        IconProjectSerializer serializer = new();

        if (model.Frames.Count == 1)
        {
            var result = await Router.Navigate<List<Point>>("/exportIcon");

            if (result.HasValue)
            {
                serializer.IconFormats = result.Value;
            }
        }
        var stream = await file.OpenWriteAsync();
        serializer.Serialize(stream, model, true);
    }

    private static IPixedProjectSerializer? GetSerializer(string format)
    {
        return format switch
        {
            ".pixed" => new PixedProjectSerializer(),
            ".png" => new PngProjectSerializer(),
            ".piskel" => new PiskelProjectSerializer(),
            ".ico" => new IconProjectSerializer(),
            ".svg" => new SvgProjectSerializer(),
            _ => null,
        };
    }
}
