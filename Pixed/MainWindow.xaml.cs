using Pixed.Controls;
using Pixed.Services;
using Pixed.Services.Keyboard;
using Pixed.Services.Palette;
using Pixed.Tools;
using Pixed.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pixed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PaintCanvas _paintCanvas;
        private readonly MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _paintCanvas = paintCanvas;
            _viewModel = (MainViewModel)DataContext;
            Initialize();
            _viewModel.Initialize(_paintCanvas.ViewModel);
        }

        private void Initialize()
        {
            Global.ShortcutService = new ShortcutService();
            Global.PaletteService = new PaletteService();
            Global.SelectionManager = new Selection.SelectionManager(ov =>
            {
                _paintCanvas.ViewModel.Overlay = ov;
            });
            Global.ToolSelector = new ToolSelector(SelectTool);
            Global.ToolSelector.SelectTool("tool_pen");
        }

        private void SelectTool(string name)
        {
            var obj = FindName(name);

            if (obj is RadioButton radioButton)
            {
                radioButton.IsChecked = true;
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
    }
}