using Pixed.Core.IO;
using Pixed.Core.Utils;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Pixed.Core.Models;

public class Frame : PixelImage, IPixedSerializer
{
    private readonly ObservableCollection<Layer> _layers;
    private int _selectedLayer = 0;
    private readonly string _id;
    private static readonly object _lock = new();

    public override int Width => Layers[0].Width;
    public override int Height => Layers[0].Height;
    public Layer CurrentLayer => Layers[SelectedLayer];
    public int SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            _selectedLayer = Math.Clamp(value, 0, Layers.Count - 1);
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Layer> Layers => _layers;

    public string Id => _id;

    public Frame(int width, int height)
    {
        _id = Guid.NewGuid().ToString();
        _layers = [];
        AddLayer(new Layer(width, height));
    }

    public static Frame FromLayers(ObservableCollection<Layer> layers)
    {
        Frame frame = new(layers[0].Width, layers[0].Height);
        frame.Layers.Clear();

        foreach (var layer in layers)
        {
            frame.AddLayer(layer);
        }

        return frame;
    }

    public Frame Clone()
    {
        Frame frame = new(Width, Height);
        frame._layers.Clear();

        foreach (Layer layer in _layers)
        {
            frame._layers.Add(layer.Clone());
        }

        return frame;
    }

    public SKCanvas GetCanvas()
    {
        return CurrentLayer.GetCanvas();
    }

    public uint GetPixel(Point point)
    {
        return CurrentLayer.GetPixel(point);
    }

    public Layer AddLayer(Layer layer)
    {
        string name = "Layer " + _layers.Count;
        layer.Name = name;
        _layers.Add(layer);
        OnPropertyChanged(nameof(Layers));
        return layer;
    }

    public override void SetModifiedPixels(List<Pixel> modifiedPixels)
    {
        CurrentLayer.SetModifiedPixels(modifiedPixels);
    }

    public override SKBitmap Render()
    {
        lock (_lock)
        {
            SKBitmap render = new(Width, Height);
            SKCanvas canvas = new(render);
            canvas.Clear(SKColors.Transparent);

            for (int a = 0; a < _layers.Count; a++)
            {
                var bitmap = _layers[a].Render();

                if(!SkiaUtils.IsNull(bitmap))
                {
                    canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
                }

                bitmap.Dispose();
            }
            canvas.Dispose();
            return render;
        }
    }

    public bool ContainsPixel(Point point)
    {
        return CurrentLayer.ContainsPixel(point);
    }

    public void MoveLayerUp()
    {
        if (_selectedLayer == 0)
        {
            return;
        }

        int oldIndex = _selectedLayer;
        int newIndex = oldIndex - 1;
        Layer layer = Layers[oldIndex];
        _layers.RemoveAt(oldIndex);
        _layers.Insert(newIndex, layer);
        SelectedLayer = newIndex;
        OnPropertyChanged(nameof(Layers));
    }

    public void MoveLayerDown()
    {
        if (_selectedLayer == Layers.Count - 1)
        {
            return;
        }

        int oldIndex = _selectedLayer;
        Layer layer = _layers[oldIndex];

        _layers.Insert(oldIndex + 2, layer);
        _layers.RemoveAt(oldIndex);
        SelectedLayer = oldIndex + 1;
        OnPropertyChanged(nameof(Layers));
    }

    public Layer? MergeLayerBelow()
    {
        if (SelectedLayer >= Layers.Count - 1)
        {
            return null;
        }

        int index = SelectedLayer;
        Layer layer = _layers[index];
        Layer layer2 = _layers[index + 1];
        layer.MergeLayers(layer2);
        Layers.RemoveAt(index + 1);
        SelectedLayer = index;
        return layer2;
    }

    public void Serialize(Stream stream)
    {
        stream.WriteInt(_selectedLayer);
        stream.WriteInt(_layers.Count);
        foreach (var layer in _layers)
        {
            layer.Serialize(stream);
        }
    }

    public void Deserialize(Stream stream)
    {
        _selectedLayer = stream.ReadInt();
        _layers.Clear();
        int layersCount = stream.ReadInt();

        for (int i = 0; i < layersCount; i++)
        {
            Layer layer = new(1, 1);
            layer.Deserialize(stream);
            _layers.Add(layer);
        }
    }
}
