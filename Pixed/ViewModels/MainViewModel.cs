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
                _selectedLayer = value;
                Frames[_selectedFrame].SelectedLayer = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddLayerCommand { get; }

        public MainViewModel()
        {
            Global.Models.Add(new PixedModel());
            Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
            OnPropertyChanged(nameof(Layers));
            AddLayerCommand = new ActionCommand(AddLayerCommandAction);
        }

        public void Initialize(PaintCanvasViewModel paintCanvas)
        {
            _paintCanvas = paintCanvas;
            _paintCanvas.CurrentFrame = Frames[_selectedFrame];
        }

        private void AddLayerCommandAction()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Frames[_selectedFrame].AddLayer(Layers[_selectedFrame].Clone());
            }
            else
            {
                Frames[_selectedFrame].AddLayer(new Layer(Frames[_selectedFrame].Width, Frames[_selectedFrame].Height));
            }

            SelectedLayer = Frames[_selectedFrame].Layers.Count - 1;
        }
    }
}
