using Pixed.Controls;
using Pixed.Models;
using Pixed.Services.Keyboard;
using Pixed.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Frame = Pixed.Models.Frame;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Drawing.Rectangle;

namespace Pixed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PaintCanvas _paintCanvas;
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