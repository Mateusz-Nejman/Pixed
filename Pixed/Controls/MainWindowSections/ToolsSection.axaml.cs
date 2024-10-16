using Pixed.ViewModels;

namespace Pixed.Controls.MainWindowSections;

internal partial class ToolsSection : PixedUserControl<ToolsSectionViewModel>
{
    public ToolsSection() : base()
    {
        InitializeComponent();
        ViewModel.InitializeTools(toolStack);
    }
}