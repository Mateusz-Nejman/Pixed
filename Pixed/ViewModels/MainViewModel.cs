using Pixed.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        private int _selectedFrame = 0;
        private PaintCanvasViewModel? _paintCanvas;
        private bool _removeFrameVisibility = false;

        public bool RemoveFrameVisibility
        {
            get => _removeFrameVisibility;
            set
            {
                _removeFrameVisibility = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Frame> Frames => Global.CurrentModel.Frames;

        public int SelectedFrame
        {
            get => _selectedFrame;
            set
            {
                _selectedFrame = Math.Clamp(value, 0, Frames.Count);
                Global.CurrentFrameIndex = _selectedFrame;
                _paintCanvas.CurrentFrame = Frames[_selectedFrame];
                Subjects.LayerChanged.OnNext(0);
                OnPropertyChanged();
                Subjects.RefreshCanvas.OnNext(true);
            }
        }

        public ICommand NewFrameCommand { get; }
        public ICommand RemoveFrameCommand { get; }
        public ICommand DuplicateFrameCommand { get; }

        public MainViewModel()
        {
            Global.Models.Add(new PixedModel());
            Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
            RemoveFrameVisibility = Frames.Count != 1;
            Subjects.FrameChanged.OnNext(Frames.Count - 1);

            NewFrameCommand = new ActionCommand(NewFrameAction);
            RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
            DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);

            Subjects.FrameChanged.Subscribe(f =>
            {
                OnPropertyChanged(nameof(Frames));
            });
        }

        public void Initialize(PaintCanvasViewModel paintCanvas)
        {
            _paintCanvas = paintCanvas;
            _paintCanvas.CurrentFrame = Frames[_selectedFrame];
            Subjects.PaletteSelected.OnNext(Global.PaletteService.Palettes[1]);
        }

        private void NewFrameAction()
        {
            Frames.Add(new Frame(Frames[0].Width, Frames[0].Height));
            SelectedFrame = Frames.Count - 1;
            RemoveFrameVisibility = Frames.Count != 1;
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
            RemoveFrameVisibility = Frames.Count != 1;
        }

        private void DuplicateFrameAction()
        {
            Frames.Add(Frames[SelectedFrame].Clone());
            SelectedFrame = Frames.Count - 1;
            RemoveFrameVisibility = Frames.Count != 1;
        }
    }
}
