using Pixed.IO;
using Pixed.Utils;
using Pixed.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Pixed.Models;

internal class PixedModel : PropertyChangedBase, IPixedSerializer
{
    private readonly ApplicationData _applicationData;
    private const int MAX_HISTORY_ENTRIES = 500;
    private readonly ObservableCollection<Frame> _frames;
    private readonly ObservableCollection<byte[]> _history;
    private int _historyIndex = 0;
    private PixedImage _renderSource;
    private int _currentFrameIndex = 0;
    private bool _isEmpty = true;

    public string? FilePath { get; set; }
    public string FileName { get; set; } = string.Empty;

    public ObservableCollection<Frame> Frames => _frames;
    public int Width => Frames[0].Width;
    public int Height => Frames[0].Height;

    public Frame CurrentFrame => Frames[_currentFrameIndex];

    public int CurrentFrameIndex
    {
        get => _currentFrameIndex;
        set
        {
            _currentFrameIndex = value;
            OnPropertyChanged();
        }
    }

    public PixedImage RenderSource
    {
        get => _renderSource;
        set
        {
            _renderSource = value;
            OnPropertyChanged();
        }
    }

    public bool IsEmpty => _isEmpty;
    public bool UnsavedChanges { get; set; } = false;

    public ICommand CloseCommand { get; }

    public PixedModel(ApplicationData applicationData) : this(applicationData, applicationData.UserSettings.UserWidth, applicationData.UserSettings.UserHeight)
    {

    }

    public PixedModel(ApplicationData applicationData, int width, int height)
    {
        _applicationData = applicationData;
        _frames = [];
        _history = [];

        CloseCommand = new ActionCommand(CloseCommandAction);

        Frames.Add(new Frame(width, height));
    }

    public void CopyHistoryFrom(PixedModel model)
    {
        _history.AddRange(model._history);
    }

    public PixedModel Clone()
    {
        PixedModel model = new(_applicationData)
        {
            _isEmpty = _isEmpty,
            _currentFrameIndex = _currentFrameIndex,
            FileName = FileName,
        };

        foreach (Frame frame in Frames)
        {
            model._frames.Add(frame.Clone());
        }

        return model;
    }

    public void UpdatePreview()
    {
        RenderSource = new PixedImage(Frames.First().Render());
    }

    public void Process(bool allFrames, bool allLayers, Action<Frame, Layer> action, ApplicationData applicationData, bool executeSubjects = true)
    {
        Frame[] frames = allFrames ? [.. Frames] : [applicationData.CurrentFrame];

        foreach (Frame frame in frames)
        {
            Layer[] layers = allLayers ? frame.Layers.ToArray() : [frame.CurrentLayer];

            foreach (Layer layer in layers)
            {
                action.Invoke(frame, layer);
            }

            if (executeSubjects)
            {
                Subjects.FrameModified.OnNext(frame);
            }
        }
    }

    public List<uint> GetAllColors()
    {
        uint transparentColor = UniColor.Transparent;
        return [.. _frames.SelectMany(f => f.Layers).Select(l => l.GetDistinctPixels()).SelectMany(p => p).Distinct().Where(p => p != transparentColor).Order()];
    }

    public void AddHistory(bool setIsEmpty = true)
    {
        _historyIndex = Math.Clamp(_historyIndex, 0, _history.Count);
        MemoryStream stream = new();
        Serialize(stream);
        byte[] data = stream.ToArray();
        stream.Dispose();

        ObservableCollection<byte[]> newHistory = [];

        if (_history.Count > 0)
        {
            for (int a = 0; a <= _historyIndex; a++)
            {
                newHistory.Add(_history[a]);
            }
        }

        newHistory.Add(data);

        _history.Clear();

        foreach (var historyData in newHistory)
        {
            _history.Add(historyData);
        }

        if (_history.Count == MAX_HISTORY_ENTRIES + 1)
        {
            _history.RemoveAt(0);
        }
        _historyIndex = _history.Count - 1;

        if (setIsEmpty)
        {
            _isEmpty = false;
        }

        UnsavedChanges = true;
    }

    public void Undo()
    {
        _historyIndex--;
        if (_history.Count == 1 || _historyIndex < 0)
        {
            return;
        }

        _historyIndex = Math.Clamp(_historyIndex, 0, _history.Count - 1);

        byte[] data = _history[_historyIndex];
        MemoryStream stream = new(data);
        Deserialize(stream);
        Subjects.ProjectModified.OnNext(this);
        Subjects.FrameChanged.OnNext(CurrentFrame);
        Subjects.LayerChanged.OnNext(CurrentFrame.CurrentLayer);
        stream.Dispose();
    }

    public void Redo()
    {
        _historyIndex++;
        if (_history.Count == 0 || _historyIndex >= _history.Count)
        {
            return;
        }

        if (_historyIndex < 0)
        {
            _historyIndex = 0;
        }

        byte[] data = _history[_historyIndex];
        MemoryStream stream = new(data);
        Deserialize(stream);
        Subjects.ProjectModified.OnNext(this);
        Subjects.FrameChanged.OnNext(CurrentFrame);
        Subjects.LayerChanged.OnNext(CurrentFrame.CurrentLayer);
        stream.Dispose();
    }

    public void Serialize(Stream stream)
    {
        stream.WriteInt(_currentFrameIndex);
        stream.WriteInt(_frames.Count);
        foreach (var frame in _frames)
        {
            frame.Serialize(stream);
        }
    }

    public void Deserialize(Stream stream)
    {
        _isEmpty = false;
        _currentFrameIndex = stream.ReadInt();
        _frames.Clear();
        int framesCount = stream.ReadInt();

        for (int i = 0; i < framesCount; i++)
        {
            Frame frame = new(1, 1);
            frame.Deserialize(stream);
            _frames.Add(frame);
        }
    }

    public static PixedModel FromFrames(ObservableCollection<Frame> frames, string name, ApplicationData applicationData)
    {
        PixedModel model = new(applicationData);
        model.Frames.Clear();
        model._isEmpty = false;
        model.FileName = name;

        foreach (var frame in frames)
        {
            model.Frames.Add(frame);
        }

        return model;
    }

    private void CloseCommandAction()
    {
        if (_applicationData.Models.Count == 1)
        {
            MainWindow.QuitCommand.Execute(null);
        }
        else
        {
            _applicationData.Models.Remove(this);
            Subjects.ProjectRemoved.OnNext(this);
            _applicationData.CurrentModelIndex = Math.Clamp(_applicationData.CurrentModelIndex, 0, _applicationData.Models.Count - 1);
        }
    }
}
