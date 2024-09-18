using Pixed.Controls;
using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class FramesSectionViewModel : PropertyChangedBase, IPixedViewModel
{
    private int _selectedFrame = 0;
    private bool _removeFrameEnabled = false;

    public static ObservableCollection<Frame> Frames => Global.CurrentModel.Frames;

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
            _selectedFrame = Math.Clamp(value, 0, Frames.Count);
            Global.CurrentFrameIndex = _selectedFrame;
            Subjects.FrameChanged.OnNext(Frames[_selectedFrame]);
            Subjects.LayerChanged.OnNext(Frames[_selectedFrame].Layers[0]);
            OnPropertyChanged();
            Subjects.RefreshCanvas.OnNext(null);
        }
    }

    public ICommand NewFrameCommand { get; }
    public ICommand RemoveFrameCommand { get; }
    public ICommand DuplicateFrameCommand { get; }

    public FramesSectionViewModel()
    {
        Subjects.FrameChanged.OnNext(Global.CurrentFrame);

        NewFrameCommand = new ActionCommand(NewFrameAction);
        RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
        DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);

        Subjects.FrameChanged.Subscribe(f =>
        {
            OnPropertyChanged(nameof(Frames));
        });
        Subjects.FrameModified.Subscribe(f =>
        {
            f.RefreshRenderSource();
            Subjects.RefreshCanvas.OnNext(null);
            OnPropertyChanged(nameof(Frames));
        });

        Subjects.FrameAdded.Subscribe(_ => RemoveFrameEnabled = Global.CurrentModel.Frames.Count != 1);
        Subjects.FrameRemoved.Subscribe(_ => RemoveFrameEnabled = Global.CurrentModel.Frames.Count != 1);
    }

    public void RegisterMenuItems()
    {
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "New Frame", NewFrameCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Duplicate Frame", DuplicateFrameCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Remove Frame", RemoveFrameCommand);
    }

    private void NewFrameAction()
    {
        Frames.Add(new Frame(Frames[0].Width, Frames[0].Height));
        SelectedFrame = Frames.Count - 1;
        Subjects.FrameAdded.OnNext(Frames[^1]);
    }

    private void RemoveFrameAction()
    {
        if (Frames.Count == 1)
        {
            return;
        }

        int index = SelectedFrame;
        Frames.RemoveAt(index);
        SelectedFrame = Math.Clamp(index, 0, Frames.Count - 1);
        Subjects.FrameRemoved.OnNext(Frames[^1]);
    }

    private void DuplicateFrameAction()
    {
        Frames.Add(Frames[SelectedFrame].Clone());
        SelectedFrame = Frames.Count - 1;
        Subjects.FrameAdded.OnNext(Frames[^1]);
    }
}
