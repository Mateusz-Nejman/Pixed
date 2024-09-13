using Avalonia.Controls;
using Pixed.ViewModels;

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
                if (select)
                {
                    Global.PaletteService.Select(model);
                    Close(true);
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
