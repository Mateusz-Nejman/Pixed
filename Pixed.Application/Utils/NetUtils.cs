using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
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
    public static async Task<IPAddress?> GetIpAddress()
    {
        var hostName = Dns.GetHostName();
        IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);

        if (localhost.AddressList.Length > 0)
        {
            return localhost.AddressList[0];
        }

        return null;
    }
    public static async Task<string[]> FindLocalServers()
    {
        var localIpAddress = await GetIpAddress();

        if(localIpAddress == null)
        {
            return [];
        }

        string subnetMask = GetSubnetMask();

        string networkAddress = CalculateNetworkAddress(localIpAddress.ToString(), subnetMask);

        string[] localServers = FindServersOnNetwork(networkAddress);

        return localServers;
    }

    private static string GetSubnetMask()
    {
        string subnetMask = "";

        // Get the network interfaces.
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            // Check if the interface is up and not a loopback or virtual interface.
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                // Get the IP properties of the interface.
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                // Get the unicast addresses.
                UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                {
                    // Check if the address is IPv4 and not a link-local or loopback address.
                    if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(unicastAddress.Address) &&
                        !unicastAddress.Address.IsIPv6LinkLocal)
                    {
                        subnetMask = unicastAddress.IPv4Mask.ToString();
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(subnetMask))
            {
                break;
            }
        }

        return subnetMask;
    }

    private static string CalculateNetworkAddress(string ipAddress, string subnetMask)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        IPAddress mask = IPAddress.Parse(subnetMask);

        byte[] ipBytes = ip.GetAddressBytes();
        byte[] maskBytes = mask.GetAddressBytes();

        byte[] networkBytes = new byte[ipBytes.Length];

        for (int i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        return new IPAddress(networkBytes).ToString();
    }

    private static string[] FindServersOnNetwork(string networkAddress)
    {
        // Create a list to store the local servers.
        List<string> localServers = [];

        // Get the local subnet.
        string localSubnet = networkAddress[..networkAddress.LastIndexOf('.')];

        // Iterate over all possible IP addresses in the subnet.
        for (int i = 1; i <= 255; i++)
        {
            string ipAddress = localSubnet + "." + i.ToString();

            // Ping the IP address to check if it is reachable.
            Ping ping = new();
            PingReply reply = ping.Send(ipAddress);

            if (reply.Status == IPStatus.Success)
            {
                localServers.Add(ipAddress);
            }
        }

        return [.. localServers];
    }
}