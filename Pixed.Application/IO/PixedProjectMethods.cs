using Avalonia.Platform.Storage;
using Pixed.Application.Models;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Services;
using Pixed.Application.Utils;
using Pixed.BigGustave;
using Pixed.Common;
using Pixed.Common.Services;
using Pixed.Core.Models;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.Application.IO;
internal class PixedProjectMethods(ApplicationData applicationData, DialogUtils dialogUtils, IStorageProviderHandle storageProvider, IHistoryService historyService)
{
    private readonly ApplicationData _applicationData = applicationData;
    private readonly DialogUtils _dialogUtils = dialogUtils;
    private readonly IStorageProviderHandle _storageProvider = storageProvider;
    private readonly IHistoryService _historyService = historyService;
    private readonly IPixedProjectSerializer[] _serializers = [new PixedProjectSerializer(), new PixiProjectSerializer(), new PiskelProjectSerializer(), new PngProjectSerializer(), new IconProjectSerializer(), new SvgProjectSerializer()];

    public async Task Save(PixedModel model, bool saveAs, RecentFilesService recentFilesService)
    {
        if (_storageProvider.StorageProvider == null)
        {
            return;
        }
        
        Stream? fileStream = null;
        PixedProjectSerializer serializer = GetSerializer<PixedProjectSerializer>();

        if (model.FilePath == null)
        {
            saveAs = true;
        }
        else if (!saveAs)
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

            var file = await _dialogUtils.SaveFileDialog(GetFilter(serializer), name);

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
            serializer.Serialize(fileStream, model, true);
            await recentFilesService.AddRecent(model.FilePath);
            model.UnsavedChanges = false;
            Subjects.ProjectChanged.OnNext(model);
        }
    }

    public async Task Open(RecentFilesService recentFilesService)
    {
        var files = await _dialogUtils.OpenFileDialog(GetFilter(_serializers), "Open file", true);

        foreach (var item in files)
        {
            var stream = await item.OpenRead();

            if (stream == null)
            {
                continue;
            }
            
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
                await stream.DisposeAsync();
                stream = await item.OpenRead();

                if (stream == null)
                {
                    continue;
                }
                
                svg.Dispose();
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
            catch (Exception)
            {
                await Router.Message("Opening error", "Invalid format");
                continue;
            }

            await stream.DisposeAsync();
            model.FileName = item.Name.Replace(item.GetExtension(), ".pixed");
            _historyService.Register(model);
            await _historyService.AddToHistory(model);

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
        if (_storageProvider.StorageProvider == null)
        {
            return;
        }
        
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
        catch (Exception)
        {
            await Router.Message("Opening error", "Invalid format");
            return;
        }
        stream.Dispose();
        model.FileName = file.Name.Replace(".png", ".pixed");
        _historyService.Register(model);
        await _historyService.AddToHistory(model);

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

    public async Task Open(Stream stream)
    {
        PixedModel model = new(_applicationData);
        try
        {
            stream.Position = 0;
            model.Deserialize(stream);
        }
        catch (Exception)
        {
            await Router.Message("Opening error", "Invalid format");
            return;
        }

        model.FilePath = null;
        
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

    public async Task Create(Png png)
    {
        PngProjectSerializer serializer = GetSerializer<PngProjectSerializer>();

        var result = await Router.Navigate<OpenPngResult>("/openPng");

        if (result.HasValue)
        {
            if (result.Value.IsTileset)
            {
                serializer.TileWidth = result.Value.TileWidth;
                serializer.TileHeight = result.Value.TileHeight;
            }
        }
        else
        {
            return;
        }

        PixedModel model;
        try
        {
            model = serializer.Deserialize(png, _applicationData);
        }
        catch (Exception)
        {
            await Router.Message("Opening error", "Invalid format");
            return;
        }

        _historyService.Register(model);
        if (_applicationData.CurrentModel.IsEmpty)
        {
            model.FileName = _applicationData.Models[_applicationData.CurrentModelIndex].FileName;
            await _historyService.AddToHistory(model);
            _applicationData.Models[_applicationData.CurrentModelIndex] = model;
        }
        else
        {
            model.FileName = _applicationData.GenerateName();
            await _historyService.AddToHistory(model);
            _applicationData.Models.Add(model);
        }

        Subjects.ProjectAdded.OnNext(model);
    }

    public async Task ExportToPng(PixedModel model)
    {
        FileInfo info = new(model.FileName);
        PngProjectSerializer serializer = GetSerializer<PngProjectSerializer>();

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
        IconProjectSerializer serializer = GetSerializer<IconProjectSerializer>();

        string name = info.Name;

        if (!string.IsNullOrEmpty(info.Extension))
        {
            name = info.Name.Replace(info.Extension, string.Empty);
        }
        if (IPlatformSettings.Instance.ExtensionsOnSave)
        {
            name += ".ico";
        }
        var file = await _dialogUtils.SaveFileDialog(GetFilter(serializer), name);

        if (file == null)
        {
            return;
        }

        if (model.Frames.Count == 1)
        {
            var result = await Router.Navigate<List<Point>>("/exportIcon");

            if (result.HasValue)
            {
                serializer.IconFormats = result.Value ?? [];
            }
        }
        var stream = await file.OpenWriteAsync();
        serializer.Serialize(stream, model, true);
    }

    private IPixedProjectSerializer? GetSerializer(string format)
    {
        foreach (var serializer in _serializers)
        {
            if (serializer.FormatExtension == format)
            {
                return serializer;
            }
        }

        return null;
    }

    private T GetSerializer<T>()
    {
        foreach (var serializer in _serializers)
        {
            if (serializer is T s)
            {
                return s;
            }
        }

        throw new NotSupportedException();
    }

    private static string GetFilter(IPixedProjectSerializer serializer)
    {
        return serializer.FormatName + " (*" + serializer.FormatExtension + ")|*" + serializer.FormatExtension;
    }

    private static string GetFilter(IPixedProjectSerializer[] serializers)
    {
        string allFormats = string.Join(';', serializers.Select(s => s.FormatExtension));
        string allFormatsStar = "*" + string.Join(";*", serializers.Select(s => s.FormatExtension));
        List<string> filters = ["All supported (" + allFormats + ")|" + allFormatsStar];

        foreach (var serializer in serializers)
        {
            filters.Add(GetFilter(serializer));
        }

        return string.Join('|', [.. filters]);
    }
}
