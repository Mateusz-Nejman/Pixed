using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Application.Utils;

internal static class NetUtils
{
    public static byte[] ToNetBytes(this string message)
    {
        return Encoding.UTF8.GetBytes(message);
    }

    public static string ToNetMessage(this byte[] data, int bytesRead = -1)
    {
        if(bytesRead == -1)
        {
            bytesRead = data.Length;
        }

        return Encoding.UTF8.GetString(data, 0, bytesRead);
    }
    public static IPAddress? GetIpAddress()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                 networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
            {
                var properties = networkInterface.GetIPProperties();
                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.Address;
                    }
                }
            }
        }
        return null;
    }
}