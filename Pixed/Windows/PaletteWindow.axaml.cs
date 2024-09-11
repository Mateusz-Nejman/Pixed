using Pixed.Models;
using Pixed.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pixed
{
    /// <summary>
    /// Interaction logic for PaletteWindow.xaml
    /// </summary>
    public partial class PaletteWindow : Window
    {
        private PaletteWindowViewModel _viewModel;
        public PaletteWindow()
        {
            InitializeComponent();
            _viewModel = (PaletteWindowViewModel)DataContext;
            _viewModel.PaletteAction = (select, model) =>
            {
                if(select)
                {
                    Global.PaletteService.Select(model);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    Global.PaletteService.Remove(model);
                }
            };
            _viewModel.PaletteRenameAction = (model, newName) =>
            {
                Global.PaletteService.Rename(model, newName);
            };
        }
    }
}
