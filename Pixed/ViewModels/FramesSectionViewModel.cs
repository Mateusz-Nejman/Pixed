using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Menu;
using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Pixed.Menu.MenuBuilder;

namespace Pixed.ViewModels;

internal class FramesSectionViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly MenuItemRegistry _menuItemRegistry;
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

    public FramesSectionViewModel(ApplicationData applicationData, MenuItemRegistry menuItemRegistry)
    {
        _applicationData = applicationData;
        _menuItemRegistry = menuItemRegistry;
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
        _menuItemRegistry.Register(BaseMenuItem.Project, "New Frame", NewFrameCommand);
        _menuItemRegistry.Register(BaseMenuItem.Project, "Duplicate Frame", DuplicateFrameCommand);
        _menuItemRegistry.Register(BaseMenuItem.Project, "Remove Frame", RemoveFrameCommand);
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
}
