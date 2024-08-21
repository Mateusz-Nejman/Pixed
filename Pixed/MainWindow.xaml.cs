using Pixed.Controls;
using Pixed.Services.Keyboard;
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
            _viewModel.Initialize(_paintCanvas.ViewModel);
            Initialize();
        }

        private void Initialize()
        {
            Global.ShortcutService = new ShortcutService();
            tool_pen.IsChecked = true;
        }

        private void ToolRadio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            string name = radio.Name;

            Global.ToolSelected = Global.Tool.GetTool(name);
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