using GongSolutions.Wpf.DragDrop;
using Pixed.Models;
using Pixed.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase, IDropTarget
    {
        private int _selectedFrame = 0;
        private PaintCanvasViewModel? _paintCanvas;
        private int _selectedLayer = 0;

        private bool _canLayerMoveUp = false;
        private bool _canLayerMoveDown = false;
        private bool _canLayerMerge = false;
        private bool _canLayerRemove = false;
        private Visibility _removeFrameVisibility = Visibility.Hidden;

        public bool CanLayerMoveUp
        {
            get => _canLayerMoveUp;
            set
            {
                _canLayerMoveUp = value;
                OnPropertyChanged();
            }
        }

        public bool CanLayerMoveDown
        {
            get => _canLayerMoveDown;
            set
            {
                _canLayerMoveDown = value;
                OnPropertyChanged();
            }
        }

        public bool CanLayerMerge
        {
            get => _canLayerMerge;
            set
            {
                _canLayerMerge = value;
                OnPropertyChanged();
            }
        }

        public bool CanLayerRemove
        {
            get => _canLayerRemove;
            set
            {
                _canLayerRemove = value;
                OnPropertyChanged();
            }
        }

        public Visibility RemoveFrameVisibility
        {
            get => _removeFrameVisibility;
            set
            {
                _removeFrameVisibility = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<Frame> Frames => Global.Models[0].Frames;
        public ObservableCollection<Layer> Layers => Frames[_selectedFrame].Layers;

        public int SelectedFrame
        {
            get => _selectedFrame;
            set
            {
                _selectedFrame = Math.Clamp(value, 0, Frames.Count);
                _paintCanvas.CurrentFrame = Frames[_selectedFrame];
                SelectedLayer = 0;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Layers));
                Subjects.RefreshCanvas.OnNext(true);
            }
        }

        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                int val = Math.Clamp(value, 0, Layers.Count);
                _selectedLayer = val;
                Frames[_selectedFrame].SelectedLayer = val;
                OnPropertyChanged();
                CanLayerMoveUp = _selectedLayer > 0;
                CanLayerMoveDown = _selectedLayer < Layers.Count - 1;
                CanLayerMerge = CanLayerMoveDown;
                CanLayerRemove = Layers.Count > 1;
            }
        }

        public ICommand AddLayerCommand { get; }
        public ICommand MoveLayerUpCommand { get; }
        public ICommand MoveLayerDownCommand { get; }
        public ICommand EditLayerNameCommand { get; }
        public ICommand MergeLayerCommand { get; }
        public ICommand RemoveLayerCommand { get; }
        public ICommand NewFrameCommand { get; }
        public ICommand RemoveFrameCommand { get; }
        public ICommand DuplicateFrameCommand { get; }

        public MainViewModel()
        {
            Global.Models.Add(new PixedModel());
            Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
            OnPropertyChanged(nameof(Layers));

            AddLayerCommand = new ActionCommand(AddLayerAction);
            MoveLayerUpCommand = new ActionCommand(MoveLayerUpAction);
            MoveLayerDownCommand = new ActionCommand(MoveLayerDownAction);
            EditLayerNameCommand = new ActionCommand(EditLayerNameAction);
            MergeLayerCommand = new ActionCommand(MergeLayerAction);
            RemoveLayerCommand = new ActionCommand(RemoveLayerAction);
            NewFrameCommand = new ActionCommand(NewFrameAction);
            RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
            DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);
        }

        public void Initialize(PaintCanvasViewModel paintCanvas)
        {
            _paintCanvas = paintCanvas;
            _paintCanvas.CurrentFrame = Frames[_selectedFrame];
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Frame sourceItem && dropInfo.TargetItem is Frame targetItem)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Frame sourceItem)
            {
                int oldIndex = Frames.IndexOf(sourceItem);
                int newIndex = dropInfo.UnfilteredInsertIndex;

                if (oldIndex == newIndex && dropInfo.TargetItem is Frame targetItem)
                {
                    newIndex = Frames.IndexOf(targetItem);
                }
                Frames.Insert(newIndex, sourceItem);

                if (oldIndex > newIndex)
                {
                    oldIndex++;
                }
                else
                {
                    newIndex--;
                }
                Frames.RemoveAt(oldIndex);
                SelectedFrame = newIndex;
            }
        }

        private void AddLayerAction()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Frames[_selectedFrame].AddLayer(Layers[_selectedFrame].Clone());
            }
            else
            {
                Frames[_selectedFrame].AddLayer(new Layer(Frames[_selectedFrame].Width, Frames[_selectedFrame].Height));
            }

            OnPropertyChanged(nameof(Layers));
            SelectedLayer = Frames[_selectedFrame].Layers.Count - 1;
        }

        private void MoveLayerUpAction()
        {
            if(_selectedLayer == 0)
            {
                return;
            }

            Frames[_selectedFrame].MoveLayerUp(Keyboard.Modifiers == ModifierKeys.Shift);
            OnPropertyChanged(nameof(Layers));
        }

        private void MoveLayerDownAction()
        {
            if (_selectedLayer == Layers.Count - 1)
            {
                return;
            }

            Frames[_selectedFrame].MoveLayerDown(Keyboard.Modifiers == ModifierKeys.Shift);
            OnPropertyChanged(nameof(Layers));
        }

        private void EditLayerNameAction()
        {
            string layerName = Layers[_selectedLayer].Name;

            LayerEditNameWindow window = new(layerName);

            if(window.ShowDialog() == true)
            {
                Layers[_selectedLayer].Name = window.NewName;
            }
        }

        private void MergeLayerAction()
        {
            Frames[_selectedFrame].MergeLayerBelow();
            OnPropertyChanged(nameof(Layers));
        }

        private void RemoveLayerAction()
        {
            if(Layers.Count == 1)
            {
                return;
            }

            int index = SelectedLayer;
            Layers.RemoveAt(index);
            SelectedLayer = Math.Clamp(index, 0, Layers.Count - 1);
        }

        private void NewFrameAction()
        {
            Frames.Add(new Frame(Frames[0].Width, Frames[0].Height));
            SelectedFrame = Frames.Count - 1;
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void RemoveFrameAction()
        {
            if(Frames.Count == 1)
            {
                return;
            }

            int index = SelectedFrame;
            Frames.RemoveAt(index);
            SelectedFrame = Math.Clamp(index, 0, Frames.Count - 1);
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void DuplicateFrameAction()
        {
            Frames.Add(Frames[SelectedFrame].Clone());
            SelectedFrame = Frames.Count - 1;
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
