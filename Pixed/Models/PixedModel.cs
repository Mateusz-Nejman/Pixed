using Avalonia.Media.Imaging;
using Pixed.IO;
using Pixed.Services.History;
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
    private readonly ObservableCollection<Frame> _frames;
    private readonly ObservableCollection<HistoryEntry> _history;
    private int _historyIndex = -1;
    private Bitmap _renderSource;
    private int _currentFrameIndex = 0;

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

    public Bitmap RenderSource
    {
        get => _renderSource;
        set
        {
            _renderSource = value;
            OnPropertyChanged();
        }
    }

    public ICommand CloseCommand { get; }

    public PixedModel() : this(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight)
    {

    }

    public PixedModel(int width, int height)
    {
        _frames = [];
        _history = [];

        CloseCommand = new ActionCommand(CloseCommandAction);

        Frames.Add(new Frame(width, height));
    }

    public void UpdatePreview()
    {
        RenderSource = Frames.First().Render().ToAvaloniaBitmap();
    }

    public void Process(bool allFrames, bool allLayers, Func<Frame, Layer, HistoryEntry?> action)
    {
        Frame[] frames = allFrames ? [.. Frames] : [Global.CurrentFrame];

        foreach (Frame frame in frames)
        {
            Layer[] layers = allLayers ? frame.Layers.ToArray() : [Global.CurrentLayer];

            foreach (Layer layer in layers)
            {
                var entry = action?.Invoke(frame, layer);

                if (entry.HasValue && entry.Value.OldColor.Length > 0)
                {
                    AddHistory(entry.Value);
                }

                Subjects.LayerModified.OnNext(layer);
            }

            Subjects.FrameModified.OnNext(frame);
        }
    }

    public List<int> GetAllColors()
    {
        return [.. _frames.SelectMany(f => f.Layers).Select(l => l.GetPixels()).SelectMany(p => p).Where(p => p != UniColor.Transparent).Distinct().Order()];
    }

    public static PixedModel FromFrames(ObservableCollection<Frame> frames)
    {
        PixedModel model = new();

        foreach (var frame in frames)
        {
            model.Frames.Add(frame);
        }

        return model;
    }

    public void AddHistory(HistoryEntry entry)
    {
        _history.Add(entry);
        _historyIndex = _history.Count - 1;
    }

    public void Undo()
    {
        if (_history.Count == 0 || _historyIndex < 0)
        {
            return;
        }

        HistoryEntry currentEntry = _history[_historyIndex];
        Frame? frame = Frames.FirstOrDefault(f => f.Id == currentEntry.FrameId, null);

        if (frame == null)
        {
            _history.RemoveAt(_historyIndex);
            _historyIndex--;
            return;
        }

        Layer? layer = frame.Layers.FirstOrDefault(l => l.Id == currentEntry.LayerId, null);

        if ((layer == null))
        {
            _history.RemoveAt(_historyIndex);
            _historyIndex--;
            return;
        }

        for (int a = 0; a < currentEntry.PixelX.Length; a++)
        {
            int pixelX = currentEntry.PixelX[a];
            int pixelY = currentEntry.PixelY[a];
            int oldcolor = currentEntry.OldColor[a];
            int newColor = currentEntry.NewColor[a];

            if (layer.GetPixel(pixelX, pixelY) == newColor)
            {
                layer.SetPixel(pixelX, pixelY, oldcolor);
            }
        }

        _historyIndex--;
    }

    public void Redo()
    {
        if (_history.Count == 0 || _historyIndex >= _history.Count)
        {
            return;
        }

        if (_historyIndex < 0)
        {
            _historyIndex = 0;
        }

        HistoryEntry currentEntry = _history[_historyIndex];
        Frame? frame = Frames.FirstOrDefault(f => f.Id == currentEntry.FrameId, null);

        if (frame == null)
        {
            _history.RemoveAt(_historyIndex);
            Redo();
            return;
        }

        Layer? layer = frame.Layers.FirstOrDefault(l => l.Id == currentEntry.LayerId, null);

        if ((layer == null))
        {
            _history.RemoveAt(_historyIndex);
            Redo();
            return;
        }

        for (int a = 0; a < currentEntry.PixelX.Length; a++)
        {
            int pixelX = currentEntry.PixelX[a];
            int pixelY = currentEntry.PixelY[a];
            int oldcolor = currentEntry.OldColor[a];
            int newColor = currentEntry.NewColor[a];

            if (layer.GetPixel(pixelX, pixelY) == oldcolor)
            {
                layer.SetPixel(pixelX, pixelY, newColor);
            }
        }

        _historyIndex++;
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

    private void CloseCommandAction()
    {
        if (Global.Models.Count == 1)
        {
            MainWindow.QuitCommand.Execute(null);
        }
        else
        {
            Global.Models.Remove(this);
            Subjects.ProjectRemoved.OnNext(this);
            Global.CurrentModelIndex = Math.Clamp(Global.CurrentModelIndex, 0, Global.Models.Count - 1);
        }
    }
}
