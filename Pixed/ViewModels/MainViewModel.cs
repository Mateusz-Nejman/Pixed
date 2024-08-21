using Pixed.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        private readonly int _selectedFrame = 0;
        private PaintCanvasViewModel? _paintCanvas;
        private int _selectedLayer = 0;


        public ObservableCollection<Frame> Frames => Global.Models[0].Frames;
        public ObservableCollection<Layer> Layers => Frames[_selectedFrame].Layers;

        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                int val = Math.Clamp(value, 0, Layers.Count);
                _selectedLayer = val;
                Frames[_selectedFrame].SelectedLayer = val;
                OnPropertyChanged();
            }
        }

        public ICommand AddLayerCommand { get; }
        public ICommand MoveLayerUp { get; }
        public ICommand MoveLayerDown { get; }

        public MainViewModel()
        {
            Global.Models.Add(new PixedModel());
            Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
            OnPropertyChanged(nameof(Layers));
            AddLayerCommand = new ActionCommand(AddLayerAction);
            MoveLayerUp = new ActionCommand(MoveLayerUpAction);
            MoveLayerDown = new ActionCommand(MoveLayerDownAction);
        }

        public void Initialize(PaintCanvasViewModel paintCanvas)
        {
            _paintCanvas = paintCanvas;
            _paintCanvas.CurrentFrame = Frames[_selectedFrame];
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
    }
}
