using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class FramesSectionViewModel : PropertyChangedBase
    {
        private int _selectedFrame = 0;
        private bool _removeFrameVisibility = false;

        public ObservableCollection<Frame> Frames => Global.CurrentModel.Frames;

        public bool RemoveFrameVisibility
        {
            get => _removeFrameVisibility;
            set
            {
                _removeFrameVisibility = value;
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
                Subjects.FrameChanged.OnNext(_selectedFrame);
                Subjects.LayerChanged.OnNext(0);
                OnPropertyChanged();
                Subjects.RefreshCanvas.OnNext(true);
            }
        }

        public ICommand NewFrameCommand { get; }
        public ICommand RemoveFrameCommand { get; }
        public ICommand DuplicateFrameCommand { get; }

        public FramesSectionViewModel()
        {
            Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));

            Subjects.FrameChanged.OnNext(Frames.Count - 1);

            NewFrameCommand = new ActionCommand(NewFrameAction);
            RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
            DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);

            Subjects.FrameChanged.Subscribe(f =>
            {
                OnPropertyChanged(nameof(Frames));
            });

            Subjects.FrameAdded.Subscribe(_ => RemoveFrameVisibility = Global.CurrentModel.Frames.Count != 1);
            Subjects.FrameRemoved.Subscribe(_ => RemoveFrameVisibility = Global.CurrentModel.Frames.Count != 1);
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
}
