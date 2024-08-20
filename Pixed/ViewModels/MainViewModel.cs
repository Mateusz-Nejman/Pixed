using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        private ObservableCollection<Frame> _frames;
        private int _selectedFrame = 0;
        private PaintCanvasViewModel? _paintCanvas;
        private int _selectedLayer = 0;


        public ObservableCollection<Layer> Layers => _frames[_selectedFrame].Layers;

        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
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
            _frames[_selectedFrame].AddLayer(new Layer(64, 32));
        }
    }
}
