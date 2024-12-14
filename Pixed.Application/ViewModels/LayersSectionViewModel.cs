using Pixed.Application.Controls;
using Pixed.Application.Routing;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class LayersSectionViewModel : PixedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly IMenuItemRegistry _menuItemRegistry;
    private int _selectedLayer = 0;

    private bool _canLayerMoveUp = false;
    private bool _canLayerMoveDown = false;
    private bool _canLayerMerge = false;
    private bool _canLayerRemove = false;
    private bool _disposedValue;

    private readonly IDisposable _frameChanged;
    private readonly IDisposable _layerModified;
    private readonly IDisposable _layerAdded;
    private readonly IDisposable _layerRemoved;

    public Frame Frame => _applicationData.CurrentFrame;
    public ObservableCollection<Layer> Layers => Frame.Layers;
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
            _applicationData.CurrentFrame.SelectedLayer = val;
            Subjects.LayerChanged.OnNext(_applicationData.CurrentLayer);
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
    public ICommand DuplicateLayerCommand { get; }
    public ICommand MoveLayerUpCommand { get; }
    public ICommand MoveLayerDownCommand { get; }
    public ICommand EditLayerNameCommand { get; }
    public ICommand MergeLayerCommand { get; }
    public ICommand RemoveLayerCommand { get; }

    public LayersSectionViewModel(ApplicationData applicationData, IMenuItemRegistry menuItemRegistry)
    {
        _applicationData = applicationData;
        _menuItemRegistry = menuItemRegistry;
        AddLayerCommand = new ActionCommand(AddLayerAction);
        DuplicateLayerCommand = new ActionCommand(DuplicateLayerAction);
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
            SelectedLayer = _applicationData.CurrentFrame.Layers.IndexOf(l);
        });

        _layerRemoved = Subjects.LayerRemoved.Subscribe(l =>
        {
            Subjects.FrameModified.OnNext(_applicationData.CurrentFrame);
        });
    }

    public override void RegisterMenuItems()
    {
        _menuItemRegistry.Register(BaseMenuItem.Project, "Add Layer to current frame", AddLayerCommand, null, new("avares://Pixed.Application/Resources/Icons/plus-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Edit layer name", EditLayerNameCommand, null, new("avares://Pixed.Application/Resources/Icons/pencil-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Merge with layer below", MergeLayerCommand, null, new("avares://Pixed.Application/Resources/Icons/download2-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Move layer up", MoveLayerUpCommand, null, new("avares://Pixed.Application/Resources/Icons/arrow-up-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Move layer down", MoveLayerDownCommand, null, new("avares://Pixed.Application/Resources/Icons/arrow-down-menu.png"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Remove current layer", RemoveLayerCommand, null, new("avares://Pixed.Application/Resources/Icons/bin-menu.png"));
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
        Layer layer = Frame.AddLayer(new Layer(Frame.Width, Frame.Height));
        OnPropertyChanged(nameof(Layers));
        Subjects.LayerAdded.OnNext(layer);
        Subjects.FrameModified.OnNext(Frame);
        _applicationData.CurrentModel.AddHistory();
    }

    private void DuplicateLayerAction()
    {
        Layer layer = Frame.AddLayer(Layers[_selectedLayer].Clone());
        OnPropertyChanged(nameof(Layers));
        Subjects.LayerAdded.OnNext(layer);
        Subjects.FrameModified.OnNext(Frame);
        _applicationData.CurrentModel.AddHistory();
    }
    private void MoveLayerUpAction()
    {
        if (_selectedLayer == 0)
        {
            return;
        }

        Frame.MoveLayerUp();
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
        _applicationData.CurrentModel.AddHistory();
    }

    private void MoveLayerDownAction()
    {
        if (_selectedLayer == Layers.Count - 1)
        {
            return;
        }

        Frame.MoveLayerDown();
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
        _applicationData.CurrentModel.AddHistory();
    }

    private async Task EditLayerNameAction()
    {
        string layerName = Layers[_selectedLayer].Name;

        var result = await Router.Prompt("Enter new layer name", "New layer name: ", layerName);

        if (result.HasValue)
        {
            Layers[_selectedLayer].Name = result.Value;
        }

        _applicationData.CurrentModel.AddHistory();
    }

    private void MergeLayerAction()
    {
        var currentLayer = Frame.CurrentLayer;
        var removedLayer = Frame.MergeLayerBelow();

        if (removedLayer == null)
        {
            return;
        }

        Subjects.LayerRemoved.OnNext(removedLayer);
        Subjects.LayerModified.OnNext(currentLayer);
        currentLayer.RefreshRenderSource();
        Subjects.FrameModified.OnNext(Frame);
        OnPropertyChanged(nameof(Layers));
        _applicationData.CurrentModel.AddHistory();
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
        _applicationData.CurrentModel.AddHistory();
    }
}
