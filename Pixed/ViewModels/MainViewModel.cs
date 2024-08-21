using Pixed.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        private readonly ObservableCollection<Frame> _frames;
        private readonly int _selectedFrame = 0;
        private PaintCanvasViewModel? _paintCanvas;
        private int _selectedLayer = 0;


        public ObservableCollection<Layer> Layers => _frames[_selectedFrame].Layers;

        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
                _frames[_selectedFrame].SelectedLayer = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddLayerCommand { get; }

        public MainViewModel()
        {
            _frames = [];
            _frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
            OnPropertyChanged(nameof(Layers));
            AddLayerCommand = new ActionCommand(AddLayerCommandAction);
        }

        public void Initialize(PaintCanvasViewModel paintCanvas)
        {
            _paintCanvas = paintCanvas;
            _paintCanvas.CurrentFrame = _frames[_selectedFrame];
        }

        private void AddLayerCommandAction()
        {
            _frames[_selectedFrame].AddLayer(new Layer(_frames[_selectedFrame].Width, _frames[_selectedFrame].Height));
            SelectedLayer = _frames[_selectedFrame].Layers.Count - 1;
        }
    }
}
