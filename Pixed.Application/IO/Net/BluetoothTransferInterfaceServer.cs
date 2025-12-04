using System.Threading.Tasks;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace Pixed.Application.IO.Net;

public class BluetoothTransferInterfaceServer : IProjectTransferInterfaceServer
{
    private readonly BluetoothListener _listener;

    public string DebugName => "Bluetooth";

    public BluetoothTransferInterfaceServer()
    {
        _listener = new BluetoothListener(BluetoothService.SerialPort);
    }
    
    public void Start()
    {
        _listener.Start();
    }

    public void Stop()
    {
        _listener.Stop();
    }

    public async Task<IProjectTransferClient> Accept()
    {
        return new BluetoothTransferClient(await _listener.AcceptBluetoothClientAsync());
    }
}