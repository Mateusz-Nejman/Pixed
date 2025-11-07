using Pixed.Core.IO;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;

namespace Pixed.Core.Models;

public class PixedModel : PixelImage, IPixedSerializer
{
    private readonly ObservableCollection<Frame> _frames;
    private int _currentFrameIndex = 0;
    private string _fileName = string.Empty;

    public string? FilePath { get; set; }
    public Matrix? ViewMatrix { get; set; }
    public string FileName
    {
        get => _fileName;
        set
        {
            _fileName = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Frame> Frames => _frames;
    public override int Width => Frames[0].Width;
    public override int Height => Frames[0].Height;

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

    public bool IsEmpty { get; set; }
    public bool UnsavedChanges { get; set; } = false;

    public ICommand CloseCommand { get; }
    public static Action<PixedModel> CloseCommandAction { get; set; }

    public string Id { get; }

    public PixedModel(ApplicationData applicationData) : this(applicationData.UserSettings.UserWidth, applicationData.UserSettings.UserHeight)
    {

    }

    public PixedModel(int width, int height)
    {
        Id = Guid.NewGuid().ToString();
        _frames = [];

        CloseCommand = new ActionCommand(() => CloseCommandAction(this));

        Frames.Add(new Frame(width, height));
    }

    public List<uint> GetAllColors()
    {
        uint transparentColor = UniColor.Transparent;
        return [.. _frames.SelectMany(f => f.Layers).Select(l => l.GetDistinctPixels()).SelectMany(p => p).Distinct().Where(p => p != transparentColor).Order()];
    }

    public override void SetModifiedPixels(List<Pixel> modifiedPixels)
    {
        CurrentFrame.SetModifiedPixels(modifiedPixels);
    }
    public override SKBitmap? Render()
    {
        Frame? frame = Frames.FirstOrDefault((Frame?)null);
        return frame?.Render();
    }

    public long CalculateStreamSize()
    {
        return (sizeof(int) * 2) + _frames.Sum(f => f.CalculateStreamSize());
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
        IsEmpty = false;
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

    public void ResetRecursive(bool onlyCurrent = true)
    {
        if (onlyCurrent)
        {
            CurrentFrame.CurrentLayer.ResetID();
            CurrentFrame.ResetID();
            ResetID();
        }
        else
        {
            foreach (var frame in _frames)
            {
                foreach (var layer in frame.Layers)
                {
                    layer.ResetID();
                }

                frame.ResetID();
            }

            ResetID();
        }
    }

    public static PixedModel FromFrames(ObservableCollection<Frame> frames, string name, ApplicationData applicationData)
    {
        PixedModel model = new(applicationData);
        model.Frames.Clear();
        model.IsEmpty = false;
        model.FileName = name;

        foreach (var frame in frames)
        {
            model.Frames.Add(frame);
        }

        return model;
    }
}
