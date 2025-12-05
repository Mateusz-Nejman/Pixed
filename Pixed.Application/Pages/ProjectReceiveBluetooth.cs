using Pixed.Application.IO.Net;

namespace Pixed.Application.Pages;

public partial class ProjectReceiveBluetooth : ProjectReceive
{
    protected override IProjectTransferInterfaceServer InterfaceServer => new BluetoothTransferInterfaceServer();

    protected override void AfterInitialize()
    {
        _viewModel?.IsIpAddressVisible = false;
    }
}