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

internal class LayersSectionViewModel : PropertyChangedBase, IPixedViewModel
{
    private int _selectedLayer = 0;

    private bool _canLayerMoveUp = false;
    private bool _canLayerMoveDown = false;
    private bool _canLayerMerge = false;
    private bool _canLayerRemove = false;

    public static Frame Frame => Global.CurrentFrame;
    public static ObservableCollection<Layer> Layers => Frame.Layers;
    public int SelectedLayer
    {
        get => _selectedLayer;
        set
        {
            int val = Math.Clamp(value, 0, Layers.Count);
            _selectedLayer = val;
            Frame.SelectedLayer = val;
            OnPropertyChanged();
            CanLayerMoveUp = _selectedLayer > 0;
            CanLayerMoveDown = _selectedLayer < Layers.Count - 1;
            CanLayerMerge = CanLayerMoveDown;
            CanLayerRemove = Layers.Count > 1;
            Global.CurrentLayerIndex = val;
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
        Subjects.LayerChanged.Subscribe(layer =>
        {
            SelectedLayer = Layers.IndexOf(layer);
            OnPropertyChanged(nameof(Layers));
        });

        Subjects.FrameChanged.Subscribe(frame =>
        {
            OnPropertyChanged(nameof(Layers));
            SelectedLayer = 0;
        });

        AddLayerCommand = new ActionCommand(AddLayerAction);
        MoveLayerUpCommand = new ActionCommand(MoveLayerUpAction);
        MoveLayerDownCommand = new ActionCommand(MoveLayerDownAction);
        EditLayerNameCommand = new AsyncCommand(EditLayerNameAction);
        MergeLayerCommand = new ActionCommand(MergeLayerAction);
        RemoveLayerCommand = new ActionCommand(RemoveLayerAction);
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

    private void AddLayerAction()
    {
        Subjects.LayerAdded.OnNext(Frame.AddLayer(Keyboard.Modifiers == KeyModifiers.Shift ? Layers[_selectedLayer].Clone() : new Layer(Frame.Width, Frame.Height)));
        OnPropertyChanged(nameof(Layers));
        SelectedLayer = Frame.Layers.Count - 1;
    }
    private void MoveLayerUpAction()
    {
        if (_selectedLayer == 0)
        {
            return;
        }

        Frame.MoveLayerUp(Keyboard.Modifiers == KeyModifiers.Shift);
        OnPropertyChanged(nameof(Layers));
    }

    private void MoveLayerDownAction()
    {
        if (_selectedLayer == Layers.Count - 1)
        {
            return;
        }

        Frame.MoveLayerDown(Keyboard.Modifiers == KeyModifiers.Shift);
        OnPropertyChanged(nameof(Layers));
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
        Subjects.LayerRemoved.OnNext(layer);
        SelectedLayer = Math.Clamp(index, 0, Layers.Count - 1);
    }
}
