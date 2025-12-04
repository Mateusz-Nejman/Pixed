using System;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;

namespace Pixed.Application.IO.Net;

public class BluetoothTransferInterfaceServer : IProjectTransferInterfaceServer
{
    private readonly Guid _listenerGuid = new ("4f5603ac-bf9b-4f63-8668-4350436bddba");
    private readonly BluetoothListener _listener;

    public string DebugName => "Bluetooth";

    public BluetoothTransferInterfaceServer()
    {
        _listener = new BluetoothListener(_listenerGuid);
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