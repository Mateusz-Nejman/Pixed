using Pixed.Application.Controls;
using Pixed.Application.Routing;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Services;
using Pixed.Core;
using Pixed.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pixed.Application.ViewModels;

internal class LayersSectionViewModel : ExtendedViewModel, IDisposable
{
    private readonly ApplicationData _applicationData;
    private readonly IMenuItemRegistry _menuItemRegistry;
    private readonly IHistoryService _historyService;
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

    public LayersSectionViewModel(ApplicationData applicationData, IMenuItemRegistry menuItemRegistry, IHistoryService historyService)
    {
        _applicationData = applicationData;
        _menuItemRegistry = menuItemRegistry;
        _historyService = historyService;
        AddLayerCommand = new AsyncCommand(AddLayerAction);
        DuplicateLayerCommand = new AsyncCommand(DuplicateLayerAction);
        MoveLayerUpCommand = new AsyncCommand(MoveLayerUpAction);
        MoveLayerDownCommand = new AsyncCommand(MoveLayerDownAction);
        EditLayerNameCommand = new AsyncCommand(EditLayerNameAction);
        MergeLayerCommand = new AsyncCommand(MergeLayerAction);
        RemoveLayerCommand = new AsyncCommand(RemoveLayerAction);

        _frameChanged = Subjects.FrameChanged.Subscribe(f =>
        {
            OnPropertyChanged(nameof(Layers));
            SelectedLayer = f.SelectedLayer;
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
        _menuItemRegistry.Register(BaseMenuItem.Project, "Add Layer to current frame", AddLayerCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_add_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Edit layer name", EditLayerNameCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_pen_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Merge with layer below", MergeLayerCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_layer_diagonal_sparkle_24_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Move layer up", MoveLayerUpCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_up_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Move layer down", MoveLayerDownCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_down_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Project, "Remove current layer", RemoveLayerCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_delete_48_regular.svg"));
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

    private async Task AddLayerAction()
    {
        Layer layer = Frame.AddLayer(new Layer(Frame.Width, Frame.Height));
        OnPropertyChanged(nameof(Layers));
        Subjects.LayerAdded.OnNext(layer);
        Subjects.FrameModified.OnNext(Frame);
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }

    private async Task DuplicateLayerAction()
    {
        Layer layer = Frame.AddLayer(Layers[_selectedLayer].Clone());
        OnPropertyChanged(nameof(Layers));
        Subjects.LayerAdded.OnNext(layer);
        Subjects.FrameModified.OnNext(Frame);
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }
    private async Task MoveLayerUpAction()
    {
        if (_selectedLayer == 0)
        {
            return;
        }

        Frame.MoveLayerUp();
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }

    private async Task MoveLayerDownAction()
    {
        if (_selectedLayer == Layers.Count - 1)
        {
            return;
        }

        Frame.MoveLayerDown();
        OnPropertyChanged(nameof(Layers));
        Subjects.FrameModified.OnNext(Frame);
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }

    private async Task EditLayerNameAction()
    {
        string layerName = Layers[_selectedLayer].Name;

        var result = await Router.Prompt("Enter new layer name", "New layer name: ", layerName);

        if (result.HasValue)
        {
            Layers[_selectedLayer].Name = result.Value;
        }

        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }

    private async Task MergeLayerAction()
    {
        var currentLayer = Frame.CurrentLayer;
        var removedLayer = Frame.MergeLayerBelow();

        if (removedLayer == null)
        {
            return;
        }

        Subjects.LayerRemoved.OnNext(removedLayer);
        Subjects.LayerModified.OnNext(currentLayer);
        Subjects.FrameModified.OnNext(Frame);
        OnPropertyChanged(nameof(Layers));
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }

    private async Task RemoveLayerAction()
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
        await _historyService.AddToHistory(_applicationData.CurrentModel);
    }
}
