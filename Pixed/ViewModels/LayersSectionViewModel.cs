using Avalonia.Input;
using Pixed.Controls;
using Pixed.Input;
using Pixed.Models;
using Pixed.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.ViewModels;

internal class LayersSectionViewModel : PropertyChangedBase, IPixedViewModel, IDisposable
{
    private int _selectedLayer = 0;

    private bool _canLayerMoveUp = false;
    private bool _canLayerMoveDown = false;
    private bool _canLayerMerge = false;
    private bool _canLayerRemove = false;
    private bool _disposedValue;

    private IDisposable _frameChanged;
    private IDisposable _layerModified;
    private IDisposable _layerAdded;
    private IDisposable _layerRemoved;

    public static Frame Frame => Global.CurrentFrame;
    public static ObservableCollection<Layer> Layers => Frame.Layers;
    public int SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            if (value == -1)
            {
                return;
            }
            int val = Math.Clamp(value, 0, Layers.Count);
            _selectedLayer = val;
            Frame.SelectedLayer = val;
            OnPropertyChanged();
            CanLayerMoveUp = _selectedLayer > 0;
            CanLayerMoveDown = _selectedLayer < Layers.Count - 1;
            CanLayerMerge = CanLayerMoveDown;
            CanLayerRemove = Layers.Count > 1;
            Global.CurrentFrame.SelectedLayer = val;
            Subjects.LayerChanged.OnNext(Global.CurrentLayer);
        }
    }

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

    public ICommand AddLayerCommand { get; }
    public ICommand MoveLayerUpCommand { get; }
    public ICommand MoveLayerDownCommand { get; }
    public ICommand EditLayerNameCommand { get; }
    public ICommand MergeLayerCommand { get; }
    public ICommand RemoveLayerCommand { get; }

    public LayersSectionViewModel()
    {
        AddLayerCommand = new ActionCommand(AddLayerAction);
        MoveLayerUpCommand = new ActionCommand(MoveLayerUpAction);
        MoveLayerDownCommand = new ActionCommand(MoveLayerDownAction);
        EditLayerNameCommand = new AsyncCommand(EditLayerNameAction);
        MergeLayerCommand = new ActionCommand(MergeLayerAction);
        RemoveLayerCommand = new ActionCommand(RemoveLayerAction);

        _frameChanged = Subjects.FrameChanged.Subscribe(f =>
        {
            OnPropertyChanged(nameof(Layers));
            SelectedLayer = f.SelectedLayer;
        });

        _layerModified = Subjects.LayerModified.Subscribe(l =>
        {
            l.Rerender();
        });

        _layerAdded = Subjects.LayerAdded.Subscribe(l =>
        {
            SelectedLayer = Global.CurrentFrame.Layers.IndexOf(l);
        });

        _layerRemoved = Subjects.LayerRemoved.Subscribe(l =>
        {
            Subjects.FrameModified.OnNext(Global.CurrentFrame);
        });
    }

    public void RegisterMenuItems()
    {
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Add Layer to current frame", AddLayerCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Edit layer name", EditLayerNameCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Merge with layer below", MergeLayerCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Move layer up", MoveLayerUpCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Move layer down", MoveLayerDownCommand);
        PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Project, "Remove current layer", RemoveLayerCommand);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _frameChanged?.Dispose();
                _layerModified?.Dispose();
                _layerAdded?.Dispose();
                _layerRemoved?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void AddLayerAction()
    {
        Layer layer = Frame.AddLayer(Keyboard.Modifiers.HasFlag(KeyModifiers.Shift) ? Layers[_selectedLayer].Clone() : new Layer(Frame.Width, Frame.Height));
        OnPropertyChanged(nameof(Layers));
        Subjects.LayerAdded.OnNext(layer);
        Subjects.FrameModified.OnNext(Frame);
    }
    private void MoveLayerUpAction()
    {
        if (_selectedLayer == 0)
        {
            return;
        }

        Frame.MoveLayerUp(Keyboard.Modifiers.HasFlag(KeyModifiers.Shift));
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
    }

    private void MoveLayerDownAction()
    {
        if (_selectedLayer == Layers.Count - 1)
        {
            return;
        }

        Frame.MoveLayerDown(Keyboard.Modifiers.HasFlag(KeyModifiers.Shift));
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
    }

    private async Task EditLayerNameAction()
    {
        string layerName = Layers[_selectedLayer].Name;

        Prompt window = new()
        {
            Title = "Enter new layer name",
            Text = "New layer name:",
            DefaultValue = layerName
        };

        if (await window.ShowDialog<bool>(MainWindow.Handle) == true)
        {
            Layers[_selectedLayer].Name = window.Value;
        }
    }

    private void MergeLayerAction()
    {
        Frame.MergeLayerBelow();
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
    }

    private void RemoveLayerAction()
    {
        if (Layers.Count == 1)
        {
            return;
        }

        int index = SelectedLayer;
        var layer = Layers[index];
        Layers.RemoveAt(index);
        SelectedLayer = Math.Clamp(index, 0, Layers.Count - 1);
        Subjects.LayerRemoved.OnNext(layer);
    }
}
