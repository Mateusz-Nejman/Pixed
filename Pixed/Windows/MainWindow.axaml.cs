﻿using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Pixed.Controls;
using Pixed.Input;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Tools;
using Pixed.ViewModels;
using System;
using System.IO;

namespace Pixed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PaintCanvas _paintCanvas;
        private readonly MainViewModel _viewModel;

        public static Window? Handle { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Handle = this;
            _paintCanvas = paintCanvas;
            _viewModel = (MainViewModel)DataContext;
            Initialize();
            _viewModel.Initialize(_paintCanvas.ViewModel);

            AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
            AddHandler(PointerReleasedEvent, MouseUpHandler, handledEventsToo: true);
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object? sender, KeyEventArgs e)
        {
            Keyboard.Modifiers = e.KeyModifiers;
            Keyboard.ProcessReleased(e.Key);
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            Keyboard.Modifiers = e.KeyModifiers;
            Keyboard.ProcessPressed(e.Key);
        }

        private void Initialize()
        {
            Global.DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pixed");

            if (!Directory.Exists(Global.DataFolder))
            {
                Directory.CreateDirectory(Global.DataFolder);
            }

            if (!Directory.Exists(Path.Combine(Global.DataFolder, "Palettes")))
            {
                Directory.CreateDirectory(Path.Combine(Global.DataFolder, "Palettes"));
            }

            Global.ShortcutService = new ShortcutService();
            Global.PaletteService = new PaletteService();
            Global.PaletteService.LoadAll();
            Global.SelectionManager = new Selection.SelectionManager(ov =>
            {
                _paintCanvas.ViewModel.Overlay = ov;
            });
            Global.ToolSelector = new ToolSelector(SelectTool);
            Global.ToolSelector.SelectTool("tool_pen");
        }

        private void SelectTool(string name)
        {
            var obj = this.FindControl<RadioButton>(name);
 
            if (obj != null)
            {
                obj.IsChecked = true;
            }
        }

        private void ToolRadio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            string name = radio.Name;

            Global.ToolSelected = Global.ToolSelector.GetTool(name);
            _paintCanvas.ViewModel.ResetOverlay();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Subjects.KeyState.OnNext(new KeyState(
                e.Key,
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)));
        }

        private void MouseUpHandler(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            ProcessMouse(point);
        }

        private void MouseDownHandler(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            ProcessMouse(point);
        }

        private void ProcessMouse(PointerPoint point)
        {
            var newLeftButton = point.Properties.IsLeftButtonPressed ? MouseButtonState.Pressed : MouseButtonState.Released;
            var newRightButton = point.Properties.IsRightButtonPressed ? MouseButtonState.Pressed : MouseButtonState.Released;
            var newMiddleButton = point.Properties.IsMiddleButtonPressed ? MouseButtonState.Pressed : MouseButtonState.Released;

            if(newLeftButton != Mouse.LeftButton)
            {
                Mouse.ButtonChanged = MouseButton.Left;
            }
            else if(newRightButton != Mouse.RightButton)
            {
                Mouse.ButtonChanged = MouseButton.Right;
            }
            else if(newMiddleButton != Mouse.MiddleButton)
            {
                Mouse.ButtonChanged = MouseButton.Middle;
            }
            Mouse.LeftButton = newLeftButton;
            Mouse.MiddleButton = newMiddleButton;
            Mouse.RightButton = newRightButton;
        }
    }
}