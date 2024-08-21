using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Models
{
    internal class Frame : PropertyChangedBase
    {
        private readonly ObservableCollection<Layer> _layers;
        private int _width;
        private int _height;
        private int _selectedLayer = 0;

        public int Width => _width;
        public int Height => _height;
        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
                OnPropertyChanged();
                Subjects.RefreshCanvas.OnNext(true);
            }
        }
        public ObservableCollection<Layer> Layers => _layers;

        public Frame(int width, int height)
        {
            _layers = [];
            _width = width;
            _height = height;
            AddLayer(new Layer(width, height));
        }

        public void SetPixel(int x, int y, int color)
        {
            _layers[SelectedLayer].SetPixel(x, y, color);
            OnPropertyChanged(nameof(Layers));
        }

        public void AddLayer(Layer layer)
        {
            string name = "Layer " + _layers.Count;
            layer.Name = name;
            _layers.Add(layer);
        }

        public Bitmap Render()
        {
            return _layers[_selectedLayer].Render();
        }
    }
}
