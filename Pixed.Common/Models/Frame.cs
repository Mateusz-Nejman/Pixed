﻿using Pixed.Common.IO;
using Pixed.Common.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Pixed.Common.Models;

public class Frame : PropertyChangedBase, IPixedSerializer
{
    private readonly ObservableCollection<Layer> _layers;
    private int _selectedLayer = 0;
    private readonly string _id;
    private PixedImage _renderSource = new(null);

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

    public PixedImage RenderSource
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

    public void SetPixel(int x, int y, uint color)
    {
        CurrentLayer.SetPixel(x, y, color);
    }

    public void SetPixels(List<Pixel> pixels)
    {
        CurrentLayer.SetPixels(pixels);
    }

    public void SetPixel(int x, int y, uint color, int toolSize)
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

    public uint GetPixel(int x, int y)
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

    public void RefreshLayerRenderSources(List<Pixel>? pixels = null)
    {
        foreach (var layer in _layers)
        {
            layer.Rerender(layer == CurrentLayer ? pixels : null);
        }
    }

    public void RefreshCurrentLayerRenderSource(List<Pixel> pixels)
    {
        RenderSource?.Dispose();
        RenderSource = new PixedImage(Render(pixels));
    }

    public SKBitmap Render(List<Pixel>? pixels = null)
    {
        SKBitmap render = new(Width, Height, true);
        SKCanvas canvas = new(render);
        canvas.Clear(SKColors.Transparent);
        for (int a = 0; a < _layers.Count; a++)
        {
            _layers[a].Render(out SKBitmap bitmap, a == SelectedLayer ? pixels : null);
            canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
        }
        canvas.Dispose();

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