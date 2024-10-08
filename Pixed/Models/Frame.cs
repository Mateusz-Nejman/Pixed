﻿using Pixed.IO;
using Pixed.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Pixed.Models;

internal class Frame : PropertyChangedBase, IPixedSerializer
{
    private readonly ObservableCollection<Layer> _layers;
    private int _selectedLayer = 0;
    private readonly string _id;
    private Bitmap _renderSource;

    public int Width => Layers[0].Width;
    public int Height => Layers[0].Height;
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

    public Bitmap RenderSource
    {
        get => _renderSource;
        set
        {
            _renderSource = value;
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

    public void SetPixel(int x, int y, int color)
    {
        CurrentLayer.SetPixel(x, y, color);
    }

    public void SetPixels(List<Pixel> pixels)
    {
        CurrentLayer.SetPixels(pixels);
    }

    public void SetPixel(int x, int y, int color, int toolSize)
    {
        if (toolSize <= 1)
        {
            SetPixel(x, y, color);
            return;
        }

        var toolPoints = PaintUtils.GetToolPoints(x, y, toolSize);
        var pixels = toolPoints.Select(p => new Pixel(p.X, p.Y, color)).ToList();
        SetPixels(pixels);
    }

    public int GetPixel(int x, int y)
    {
        return CurrentLayer.GetPixel(x, y);
    }

    public Layer AddLayer(Layer layer)
    {
        string name = "Layer " + _layers.Count;
        layer.Name = name;
        layer.RefreshRenderSource();
        _layers.Add(layer);
        OnPropertyChanged(nameof(Layers));
        return layer;
    }

    public void RefreshLayerRenderSources()
    {
        foreach (var layer in _layers)
        {
            layer.Rerender();
        }
    }

    public void RefreshRenderSource(List<Pixel>? pixels = null)
    {
        RenderSource = Render(pixels).ToAvaloniaBitmap();
    }

    public System.Drawing.Bitmap Render(List<Pixel>? pixels = null)
    {
        System.Drawing.Bitmap render = new(Width, Height);
        Graphics g = Graphics.FromImage(render);
        for (int a = 0; a < _layers.Count; a++)
        {
            g.DrawImage(_layers[a].Render(a == SelectedLayer ? pixels : null), 0, 0);
        }

        return render;
    }

    public bool ContainsPixel(int x, int y)
    {
        return CurrentLayer.ContainsPixel(x, y);
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
