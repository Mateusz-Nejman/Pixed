using Pixed.Application.Controls;
using Pixed.Application.Platform;
using Pixed.Application.Utils;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class FramesSectionViewModel : ExtendedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly IMenuItemRegistry _menuItemRegistry;
    private readonly IPlatformFolder _platformFolder;
    private int _selectedFrame = 0;
    private bool _removeFrameEnabled = false;
    private bool _disposedValue;
    private readonly IDisposable _projectChanged;
    private readonly IDisposable _frameAdded;
    private readonly IDisposable _frameRemoved;
    private bool _isVisible = true;

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

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            OnPropertyChanged();
            _applicationData.UserSettings.FramesViewVisible = value;
            Task.Run(async () =>
            {
                await SettingsUtils.Save(_platformFolder, _applicationData);
            });
        }
    }

    public ICommand NewFrameCommand { get; }
    public ICommand RemoveFrameCommand { get; }
    public ICommand DuplicateFrameCommand { get; }
    public ICommand CloseViewCommand { get; }

    public FramesSectionViewModel(ApplicationData applicationData, IMenuItemRegistry menuItemRegistry, IPlatformFolder platformFolder)
    {
        _applicationData = applicationData;
        _menuItemRegistry = menuItemRegistry;
        _platformFolder = platformFolder;
        NewFrameCommand = new ActionCommand(NewFrameAction);
        RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
        DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);
        CloseViewCommand = new ActionCommand(CloseView);
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

        _isVisible = _applicationData.UserSettings.FramesViewVisible;
        OnPropertyChanged(nameof(IsVisible));
    }

    public override void RegisterMenuItems()
    {
        _menuItemRegistry.Register(BaseMenuItem.Project, "New Frame", NewFrameCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_add_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Duplicate Frame", DuplicateFrameCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_layer_diagonal_24_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Remove Frame", RemoveFrameCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_delete_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Base, "Toggle frames view", ToggleView, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_image_multiple_48_regular.svg"));
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

    private void CloseView()
    {
        IsVisible = false;
    }

    private void ToggleView()
    {
        IsVisible = !IsVisible;
    }
}
