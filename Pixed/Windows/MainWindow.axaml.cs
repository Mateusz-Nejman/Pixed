using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Controls;
using Pixed.Input;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
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

        public static Window? Handle { get; private set; }
        public MainWindow()
        {
            InitializeBeforeUI();
            InitializeComponent();

            Handle = this;
            _paintCanvas = paintCanvas;

            InitializeAfterUI();
        }

        private void InitializeBeforeUI()
        {
            Global.Models.Add(new PixedModel());
            Global.CurrentModel.Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
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

        private void InitializeAfterUI()
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

            Global.ToolSelector.SelectTool("tool_pen");

            Subjects.FrameChanged.OnNext(0);
            Subjects.PaletteSelected.OnNext(Global.PaletteService.Palettes[1]); //TODO check if needed

            toolsSection.PaintCanvas = _paintCanvas.ViewModel;

            AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
            AddHandler(PointerReleasedEvent, MouseUpHandler, handledEventsToo: true);
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;
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
            Mouse.ProcessPoint(point);
        }

        private void MouseDownHandler(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            Mouse.ProcessPoint(point);
        }
    }
}