using Avalonia.Controls;
using Avalonia.Input;
using Pixed.Input;
using Pixed.Models;

namespace Pixed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Window? Handle { get; private set; }
        public MainWindow()
        {
            Handle = this;
            InitializeBeforeUI();
            InitializeComponent();

            Global.ToolSelector.SelectTool("tool_pen");
            toolsSection.PaintCanvas = paintCanvas.ViewModel;
            Subjects.FrameChanged.OnNext(Global.CurrentFrame);

            Global.SelectionManager = new Selection.SelectionManager(ov =>
            {
                paintCanvas.ViewModel.Overlay = ov;
            });
        }

        private void InitializeBeforeUI()
        {
            Global.Models.Add(new PixedModel());
            Global.CurrentModel.Frames.Add(new Frame(Global.UserSettings.UserWidth, Global.UserSettings.UserHeight));
        }

        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {
            Keyboard.Modifiers = e.KeyModifiers;
            Keyboard.ProcessReleased(e.Key);
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            Keyboard.Modifiers = e.KeyModifiers;
            Keyboard.ProcessPressed(e.Key);
        }

        private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            Mouse.ProcessPoint(point);
        }

        private void Window_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            Mouse.ProcessPoint(point);
        }
    }
}