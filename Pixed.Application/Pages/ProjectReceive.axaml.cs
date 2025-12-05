using Pixed.Application.Utils;
using System.Threading.Tasks;
using Pixed.Application.IO.Net;
using Pixed.Application.ViewModels;

namespace Pixed.Application.Pages;

public partial class ProjectReceive : Modal
{
    protected ProjectReceiveViewModel? _viewModel;

    protected virtual IProjectTransferInterfaceServer InterfaceServer => new TcpTransferInterfaceServer();
    public ProjectReceive()
    {
        InitializeComponent();
        Initialize();
    }

    private void Initialize()
    {
        DataContext = _viewModel = new ProjectReceiveViewModel(this, InterfaceServer);
        AfterInitialize();
        Task.Run(_viewModel.TryReceiveProject);
    }

    protected virtual void AfterInitialize()
    {
        _viewModel?.IpAddress = NetUtils.GetIpAddress()?.ToString() ?? "0.0.0.0";
    }
}