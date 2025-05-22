using Pixed.Application.IO;
using Pixed.Application.Platform;
using Pixed.Common.Services;
using Pixed.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pixed.Application.Services;

internal class HistoryService(IPlatformFolder platformFolder) : IHistoryService
{
    private struct HistoryEntry(string historyId, UnmanagedArray data, UnmanagedMemoryStream stream)
    {
        public bool Cached { get; private set; }
        public string HistoryId { get; private set; } = historyId;
        public UnmanagedArray? Data { get; private set; } = data;
        public UnmanagedMemoryStream? Stream { get; private set; } = stream;
        public readonly long Size => Data == null ? 0 : Data.Length;

        public void SetCached()
        {
            Cached = true;
        }

        public void ClearData()
        {
            Data?.Dispose();
            Data = null;
            Stream?.Dispose();
            Stream = null;
        }
    }

    private const long MAX_HISTORY_SIZE = 536870912; //512 MB
    private const int MAX_HISTORY_ENTRIES = 500;
    private readonly IPlatformFolder _platformFolder = platformFolder;
    private readonly Dictionary<string, List<int>> _historyItems = [];
    private readonly Dictionary<string, int> _historyIndexes = [];
    private readonly List<HistoryEntry> _entries = [];
    private bool _cacheTask = false;

    public unsafe Task AddToHistory(PixedModel model, bool setIsEmpty = true)
    {
        if (!_historyItems.TryGetValue(model.Id, out List<int>? historyIndexes))
        {
            return Task.CompletedTask;
        }

        var historyIndex = Math.Clamp(_historyIndexes[model.Id], 0, historyIndexes.Count);

        string historyId = Guid.NewGuid().ToString();

        List<int> newHistory = [];

        if (historyIndexes.Count > 0)
        {
            for (int a = 0; a <= historyIndex; a++)
            {
                newHistory.Add(historyIndexes[a]);
            }

        }

        UnmanagedArray unmanagedArray = new((int)model.CalculateStreamSize());
        UnmanagedMemoryStream memory = new((byte*)unmanagedArray.Ptr, unmanagedArray.Length, unmanagedArray.Length, FileAccess.ReadWrite);
        model.Serialize(memory);
        memory.Position = 0;

        _entries.Add(new HistoryEntry(historyId, unmanagedArray, memory));
        newHistory.Add(_entries.Count - 1);

        historyIndexes.Clear();

        foreach (var id in newHistory)
        {
            historyIndexes.Add(id);
        }

        if (historyIndexes.Count == MAX_HISTORY_ENTRIES + 1)
        {
            historyIndexes.RemoveAt(0);
        }

        historyIndex = historyIndexes.Count - 1;

        _historyItems[model.Id] = historyIndexes;
        _historyIndexes[model.Id] = historyIndex;

        if (setIsEmpty)
        {
            model.IsEmpty = false;
        }
        model.UnsavedChanges = true;

        CheckAndProcessCache();
        return Task.CompletedTask;
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
        var arrayIndex = _historyItems[model.Id][index];

        return _entries[arrayIndex].HistoryId;
    }

    public int GetHistoryCount(PixedModel model)
    {
        return _historyItems[model.Id].Count;
    }

    public async Task<Stream> GetHistoryItem(PixedModel model, string id)
    {
        var entry = _entries.First(e => e.HistoryId == id);

        if(entry.Cached)
        {
            MemoryStream memory = new();
            var file = await _platformFolder.GetFile(id, FolderType.History);
            var stream = await file.OpenRead();
            PixedProjectSerializer.Decompress(stream, memory);
            memory.Position = 0;
            return memory;
        }

        unsafe
        {
            return new UnmanagedMemoryStream((byte*)entry.Data.Ptr, entry.Data.Length, entry.Data.Length, FileAccess.ReadWrite);
        }
    }

    public async Task ClearTempFiles()
    {
        var files = _platformFolder.GetFiles(FolderType.History);

        await foreach (var file in files)
        {
            await file.Delete();
        }
    }

    private void CheckAndProcessCache()
    {
        if (_cacheTask)
        {
            return;
        }
        _cacheTask = true;

        Task.Run(CheckAndProcessCacheAsync);
    }

    private async Task CheckAndProcessCacheAsync()
    {
        try
        {
            int maxEntries = _entries.Count;
            long size = 0;
            bool processCache = false;

            for (int a = 0; a < maxEntries; a++)
            {
                if (_entries[a].Cached)
                {
                    continue;
                }

                size += _entries[a].Size;

                if (size >= MAX_HISTORY_SIZE)
                {
                    processCache = true;
                    break;
                }
            }

            if (processCache)
            {
                for (int a = 0; a < maxEntries; a++)
                {
                    if (_entries[a].Cached || _entries[a].Data == null)
                    {
                        continue;
                    }

                    var file = await _platformFolder.GetFile(_entries[a].HistoryId, FolderType.History);
                    var stream = await file.OpenWrite();

                    _entries[a].Stream.Position = 0;
                    PixedProjectSerializer.Compress(_entries[a].Stream, stream);
                    stream.Dispose();
                    _entries[a].ClearData();
                    _entries[a].SetCached();
                }
            }
        }
        catch (Exception)
        {
            _cacheTask = false;
        }

        _cacheTask = false;
    }
}