using Pixed.Application.ViewModels;

namespace Pixed.Application.Controls.MainWindowSections;

internal partial class ToolsSection : PixedUserControl<ToolsSectionViewModel>
{
    public ToolsSection() : base()
    {
        InitializeComponent();
        ViewModel.InitializeTools(toolStack);
    }
}