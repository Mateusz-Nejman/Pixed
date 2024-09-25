using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Pixed.MenuBuilder;

namespace Pixed.ViewModels;

internal class FramesSectionViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly MenuBuilder _menuBuilder;
    private int _selectedFrame = 0;
    private bool _removeFrameEnabled = false;
    private bool _disposedValue;
    private readonly IDisposable _projectChanged;
    private readonly IDisposable _frameAdded;
    private readonly IDisposable _frameRemoved;

    public ObservableCollection<Frame> Frames => _applicationData.CurrentModel.Frames;

    public bool RemoveFrameEnabled
    {
        get => _removeFrameEnabled;
        set
        {
            _removeFrameEnabled = value;
            OnPropertyChanged();
        }
    }

    public int SelectedFrame
    {
        get => _selectedFrame;
        set
        {
            if (value == -1)
            {
                return;
            }
            _selectedFrame = Math.Clamp(value, 0, Frames.Count);
            _applicationData.CurrentModel.CurrentFrameIndex = _selectedFrame;
            OnPropertyChanged();
            Subjects.FrameChanged.OnNext(_applicationData.CurrentFrame);
        }
    }

    public ICommand NewFrameCommand { get; }
    public ICommand RemoveFrameCommand { get; }
    public ICommand DuplicateFrameCommand { get; }

    public FramesSectionViewModel(ApplicationData applicationData, MenuBuilder menuBuilder)
    {
        _applicationData = applicationData;
        _menuBuilder = menuBuilder;
        NewFrameCommand = new ActionCommand(NewFrameAction);
        RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
        DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);
        _projectChanged = Subjects.ProjectChanged.Subscribe(p =>
        {
            OnPropertyChanged(nameof(Frames));
            SelectedFrame = p.CurrentFrameIndex;
            RemoveFrameEnabled = _applicationData.CurrentModel.Frames.Count > 1;
        });

        _frameAdded = Subjects.FrameAdded.Subscribe(f =>
        {
            RemoveFrameEnabled = _applicationData.CurrentModel.Frames.Count > 1;
        });

        _frameRemoved = Subjects.FrameRemoved.Subscribe(f =>
        {
            RemoveFrameEnabled = _applicationData.CurrentModel.Frames.Count > 1;
        });
    }

    public override void RegisterMenuItems()
    {
        RegisterMenuItem(BaseMenuItem.Project, "New Frame", NewFrameCommand);
        RegisterMenuItem(BaseMenuItem.Project, "Duplicate Frame", DuplicateFrameCommand);
        RegisterMenuItem(BaseMenuItem.Project, "Remove Frame", RemoveFrameCommand);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _projectChanged?.Dispose();
                _frameAdded?.Dispose();
                _frameRemoved?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void NewFrameAction()
    {
        Frames.Add(new Frame(Frames[0].Width, Frames[0].Height));
        SelectedFrame = Frames.Count - 1;
        _applicationData.CurrentModel.AddHistory();
        Subjects.FrameAdded.OnNext(Frames[^1]);
    }

    private void RemoveFrameAction()
    {
        if (Frames.Count == 1)
        {
            return;
        }

        int index = SelectedFrame;
        var frame = Frames[index];
        Frames.RemoveAt(index);
        Subjects.FrameRemoved.OnNext(frame);
        SelectedFrame = Math.Clamp(index, 0, Frames.Count - 1);
        _applicationData.CurrentModel.AddHistory();
    }

    private void DuplicateFrameAction()
    {
        Frames.Add(Frames[SelectedFrame].Clone());
        Subjects.FrameAdded.OnNext(Frames[^1]);
        SelectedFrame = Frames.Count - 1;
        _applicationData.CurrentModel.AddHistory();
    }

    private void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
    {
        RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
    }

    private void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
    {
        _menuBuilder.AddEntry(baseMenu, menuItem);
    }
}
