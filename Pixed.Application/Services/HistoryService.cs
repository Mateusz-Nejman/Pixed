using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Common.Services;
using Pixed.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Application.Services;

internal class HistoryService(IPlatformFolder platformFolder) : IHistoryService
{
    private const int MAX_HISTORY_ENTRIES = 500;
    private readonly IPlatformFolder _platformFolder = platformFolder;
    private readonly Dictionary<string, List<string>> _historyItems = [];
    private readonly Dictionary<string, int> _historyIndexes = [];

    public async Task AddToHistory(PixedModel model, bool setIsEmpty = true)
    {
        if (!_historyItems.TryGetValue(model.Id, out List<string>? history))
        {
            return;
        }

        var historyIndex = Math.Clamp(_historyIndexes[model.Id], 0, history.Count);

        string historyId = Guid.NewGuid().ToString();

        var file = await _platformFolder.GetFile(historyId, FolderType.History);
        var stream = await file.OpenWrite();

        MemoryStream memory = new();
        model.Serialize(memory);
        memory.Position = 0;

        PixedProjectSerializer.Compress(memory, stream);
        stream.Dispose();

        List<string> newHistory = [];

        if (history.Count > 0)
        {
            for (int a = 0; a <= historyIndex; a++)
            {
                newHistory.Add(history[a]);
            }

        }

        newHistory.Add(historyId);

        history.Clear();

        foreach(var id in newHistory)
        {
            history.Add(id);
        }

        if(history.Count == MAX_HISTORY_ENTRIES + 1)
        {
            history.RemoveAt(0);
        }

        historyIndex = history.Count - 1;

        _historyItems[model.Id] = history;
        _historyIndexes[model.Id] = historyIndex;

        if (setIsEmpty)
        {
            model.IsEmpty = false;
        }
        model.UnsavedChanges = true;
    }

    public void CopyHistoryFrom(PixedModel from, PixedModel to)
    {
        _historyItems[to.Id].Clear();
        _historyItems[to.Id] = [.. _historyItems[from.Id]];
    }

    public void Register(PixedModel model)
    {
        _historyItems.Add(model.Id, []);
        _historyIndexes.Add(model.Id, 0);
    }

    public int GetHistoryIndex(PixedModel model)
    {
        return _historyIndexes[model.Id];
    }

    public void SetHistoryIndex(PixedModel model, int index)
    {
        _historyIndexes[model.Id] = index;
    }

    public string GetHistoryId(PixedModel model, int index)
    {
        return _historyItems[model.Id][index];
    }

    public int GetHistoryCount(PixedModel model)
    {
        return _historyItems[model.Id].Count;
    }

    public async Task<Stream> GetHistoryItem(PixedModel model, string id)
    {
        MemoryStream memory = new();
        var file = await _platformFolder.GetFile(id, FolderType.History);
        var stream = await file.OpenRead();
        PixedProjectSerializer.Decompress(stream, memory);
        memory.Position = 0;
        return memory;
    }

    public async Task ClearTempFiles()
    {
        var files = _platformFolder.GetFiles(FolderType.History);

        await foreach(var file in files)
        {
            await file.Delete();
        }
    }
}