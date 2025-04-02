using System.Diagnostics;

namespace Pixed.Application.Utils;
internal static class PlatformUtils
{
    public static void OpenUrl(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}