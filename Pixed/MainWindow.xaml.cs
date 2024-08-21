using Pixed.Controls;
using Pixed.Services.Keyboard;
using Pixed.ViewModels;
using System.Windows;

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
        }
    }
}