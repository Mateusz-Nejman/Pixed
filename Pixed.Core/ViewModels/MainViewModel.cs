﻿using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using Pixed.Models;
using Pixed.Tools.Transform;
using Pixed.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class MainViewModel : PropertyChangedBase, IDropTarget
    {
        private int _selectedFrame = 0;
        private PaintCanvasViewModel? _paintCanvas;
        private int _selectedLayer = 0;

        private bool _canLayerMoveUp = false;
        private bool _canLayerMoveDown = false;
        private bool _canLayerMerge = false;
        private bool _canLayerRemove = false;
        private Visibility _removeFrameVisibility = Visibility.Hidden;
        private UniColor _primaryColor = UniColor.Black;
        private UniColor _secondaryColor = UniColor.White;

        public System.Windows.Media.Color PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                OnPropertyChanged();
                Subjects.PrimaryColorChanged.OnNext(value);
            }
        }

        public System.Windows.Media.Color SecondaryColor
        {
            get => _secondaryColor;
            set
            {
                _secondaryColor = value;
                OnPropertyChanged();
                Subjects.SecondaryColorChanged.OnNext(value);
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

        public Visibility RemoveFrameVisibility
        {
            get => _removeFrameVisibility;
            set
            {
                _removeFrameVisibility = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<Frame> Frames => Global.CurrentModel.Frames;
        public ObservableCollection<Layer> Layers => Frames[_selectedFrame].Layers;

        public int SelectedFrame
        {
            get => _selectedFrame;
            set
            {
                _selectedFrame = Math.Clamp(value, 0, Frames.Count);
                Global.CurrentFrameIndex = _selectedFrame;
                _paintCanvas.CurrentFrame = Frames[_selectedFrame];
                SelectedLayer = 0;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Layers));
                Subjects.RefreshCanvas.OnNext(true);
            }
        }

        public int SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                int val = Math.Clamp(value, 0, Layers.Count);
                _selectedLayer = val;
                Frames[_selectedFrame].SelectedLayer = val;
                OnPropertyChanged();
                CanLayerMoveUp = _selectedLayer > 0;
                CanLayerMoveDown = _selectedLayer < Layers.Count - 1;
                CanLayerMerge = CanLayerMoveDown;
                CanLayerRemove = Layers.Count > 1;
                Global.CurrentLayerIndex = val;
            }
        }

        public ICommand AddLayerCommand { get; }
        public ICommand MoveLayerUpCommand { get; }
        public ICommand MoveLayerDownCommand { get; }
        public ICommand EditLayerNameCommand { get; }
        public ICommand MergeLayerCommand { get; }
        public ICommand RemoveLayerCommand { get; }

        public ICommand NewFrameCommand { get; }
        public ICommand RemoveFrameCommand { get; }
        public ICommand DuplicateFrameCommand { get; }

        public ICommand ToolFlipCommand { get; }
        public ICommand ToolRotateCommand { get; }
        public ICommand ToolCenterCommand { get; }
        public ICommand ToolCropCommand { get; }

        public PaletteModel SelectedPalette { get; private set; }
        public ObservableCollection<UniColor> SelectedPaletteColors
        {
            get
            {
                if(SelectedPalette == null)
                {
                    return null;
                }

                return new ObservableCollection<UniColor>(SelectedPalette.Colors.Select(s => (UniColor)s));
            }
        }
        public ObservableCollection<UniColor> CurrentPaletteColors
        {
            get
            {
                if(Global.PaletteService == null)
                {
                    return null;
                }
                return new ObservableCollection<UniColor>(Global.PaletteService.CurrentColorsPalette.Colors.Select(s => (UniColor)s));
            }
        }

        public ICommand PaletteAddPrimaryCommand { get; }
        public ICommand PaletteAddCurrentCommand { get; }
        public ICommand PaletteListCommand { get; }
        public ICommand PaletteOpenCommand { get; }
        public ICommand PaletteSaveCommand { get; }
        public ICommand PaletteClearCommand { get; }
        public MainViewModel()
        {
            Global.Models.Add(new PixedModel());
            Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
            OnPropertyChanged(nameof(Layers));

            AddLayerCommand = new ActionCommand(AddLayerAction);
            MoveLayerUpCommand = new ActionCommand(MoveLayerUpAction);
            MoveLayerDownCommand = new ActionCommand(MoveLayerDownAction);
            EditLayerNameCommand = new ActionCommand(EditLayerNameAction);
            MergeLayerCommand = new ActionCommand(MergeLayerAction);
            RemoveLayerCommand = new ActionCommand(RemoveLayerAction);
            NewFrameCommand = new ActionCommand(NewFrameAction);
            RemoveFrameCommand = new ActionCommand(RemoveFrameAction);
            DuplicateFrameCommand = new ActionCommand(DuplicateFrameAction);
            ToolFlipCommand = new ActionCommand(ToolFlipAction);
            ToolRotateCommand = new ActionCommand(ToolRotateAction);
            ToolCenterCommand = new ActionCommand(ToolCenterAction);
            ToolCropCommand = new ActionCommand(ToolCropAction);

            Subjects.FrameChanged.Subscribe(f =>
            {
                OnPropertyChanged(nameof(Frames));
                OnPropertyChanged(nameof(Layers));
            });

            Subjects.PrimaryColorChanged.Subscribe(c => Global.PrimaryColor = c);
            Subjects.SecondaryColorChanged.Subscribe(c => Global.SecondaryColor = c);
            Subjects.PrimaryColorChange.Subscribe(c => PrimaryColor = c);

            PrimaryColor = UniColor.Black;
            SecondaryColor = UniColor.White;

            Subjects.PaletteSelected.Subscribe(p =>
            {
                SelectedPalette = p.ToCurrentPalette();
                OnPropertyChanged(nameof(SelectedPaletteColors));
            });

            PaletteAddPrimaryCommand = new ActionCommand(PaletteAddPrimaryAction);
            PaletteAddCurrentCommand = new ActionCommand(PaletteAddCurrentAction);
            PaletteOpenCommand = new ActionCommand(PaletteOpenAction);
            PaletteSaveCommand = new ActionCommand(PaletteSaveAction);
            PaletteClearCommand = new ActionCommand(PaletteClearAction);
            PaletteListCommand = new ActionCommand(PaletteListAction);
        }

        public void Initialize(PaintCanvasViewModel paintCanvas)
        {
            _paintCanvas = paintCanvas;
            _paintCanvas.CurrentFrame = Frames[_selectedFrame];
            Subjects.PaletteSelected.OnNext(Global.PaletteService.Palettes[1]);
            Subjects.RefreshCanvas.Subscribe(_ =>
            {
                Global.PaletteService.SetCurrentColors();
                OnPropertyChanged(nameof(CurrentPaletteColors));
            });
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Frame sourceItem && dropInfo.TargetItem is Frame targetItem)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Frame sourceItem)
            {
                int oldIndex = Frames.IndexOf(sourceItem);
                int newIndex = dropInfo.UnfilteredInsertIndex;

                if (oldIndex == newIndex && dropInfo.TargetItem is Frame targetItem)
                {
                    newIndex = Frames.IndexOf(targetItem);
                }
                Frames.Insert(newIndex, sourceItem);

                if (oldIndex > newIndex)
                {
                    oldIndex++;
                }
                else
                {
                    newIndex--;
                }
                Frames.RemoveAt(oldIndex);
                SelectedFrame = newIndex;
            }
        }

        private void AddLayerAction()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Frames[_selectedFrame].AddLayer(Layers[_selectedFrame].Clone());
            }
            else
            {
                Frames[_selectedFrame].AddLayer(new Layer(Frames[_selectedFrame].Width, Frames[_selectedFrame].Height));
            }

            OnPropertyChanged(nameof(Layers));
            SelectedLayer = Frames[_selectedFrame].Layers.Count - 1;
        }

        private void MoveLayerUpAction()
        {
            if (_selectedLayer == 0)
            {
                return;
            }

            Frames[_selectedFrame].MoveLayerUp(Keyboard.Modifiers == ModifierKeys.Shift);
            OnPropertyChanged(nameof(Layers));
        }

        private void MoveLayerDownAction()
        {
            if (_selectedLayer == Layers.Count - 1)
            {
                return;
            }

            Frames[_selectedFrame].MoveLayerDown(Keyboard.Modifiers == ModifierKeys.Shift);
            OnPropertyChanged(nameof(Layers));
        }

        private void EditLayerNameAction()
        {
            string layerName = Layers[_selectedLayer].Name;

            Prompt window = new();
            window.Title = "Enter new layer name";
            window.Text = "New layer name:";
            window.DefaultValue = layerName;

            if (window.ShowDialog() == true)
            {
                Layers[_selectedLayer].Name = window.Value;
            }
        }

        private void MergeLayerAction()
        {
            Frames[_selectedFrame].MergeLayerBelow();
            OnPropertyChanged(nameof(Layers));
        }

        private void RemoveLayerAction()
        {
            if (Layers.Count == 1)
            {
                return;
            }

            int index = SelectedLayer;
            Layers.RemoveAt(index);
            SelectedLayer = Math.Clamp(index, 0, Layers.Count - 1);
        }

        private void NewFrameAction()
        {
            Frames.Add(new Frame(Frames[0].Width, Frames[0].Height));
            SelectedFrame = Frames.Count - 1;
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void RemoveFrameAction()
        {
            if (Frames.Count == 1)
            {
                return;
            }

            int index = SelectedFrame;
            Frames.RemoveAt(index);
            SelectedFrame = Math.Clamp(index, 0, Frames.Count - 1);
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void DuplicateFrameAction()
        {
            Frames.Add(Frames[SelectedFrame].Clone());
            SelectedFrame = Frames.Count - 1;
            RemoveFrameVisibility = Frames.Count == 1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void ToolFlipAction()
        {
            AbstractTransformTool flip = new Flip();
            flip.ApplyTransformation();
        }

        private void ToolRotateAction()
        {
            AbstractTransformTool rotate = new Rotate();
            rotate.ApplyTransformation();
        }

        private void ToolCenterAction()
        {
            AbstractTransformTool center = new Center();
            center.ApplyTransformation();
        }

        private void ToolCropAction()
        {
            AbstractTransformTool crop = new Crop();
            crop.ApplyTransformation();
        }

        private void PaletteAddPrimaryAction()
        {
            Global.PaletteService.AddPrimaryColor();
            OnPropertyChanged(nameof(SelectedPaletteColors));
        }

        private void PaletteAddCurrentAction()
        {
            Global.PaletteService.AddColorsFromPalette(Global.PaletteService.CurrentColorsPalette);
            OnPropertyChanged(nameof(SelectedPaletteColors));
        }

        private void PaletteClearAction()
        {
            Global.PaletteService.ClearPalette();
            OnPropertyChanged(nameof(SelectedPaletteColors));
        }
        
        private void PaletteOpenAction()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl|All Supported (.json;.gpl)|*.json;*.gpl";
            openFileDialog.FilterIndex = 3;
            if (openFileDialog.ShowDialog() == true)
            {
                Global.PaletteService.Load(openFileDialog.FileName);
            }
        }

        private void PaletteSaveAction()
        {
            if(Global.PaletteService.SelectedPalette.Colors.Count == 0)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Global.PaletteService.SelectedPalette.Name;
            saveFileDialog.Filter = "Pixed Palettes (*.json)|*.json|GIMP Palettes (*.gpl)|*.gpl|All Supported (.json;.gpl)|*.json;*.gpl";
            saveFileDialog.FilterIndex = 3;
            if (saveFileDialog.ShowDialog() == true)
            {
                Global.PaletteService.Save(saveFileDialog.FileName);
            }
        }

        private void PaletteListAction()
        {
            PaletteWindow window = new PaletteWindow();
            window.ShowDialog();
        }
    }
}