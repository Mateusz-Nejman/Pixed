using Avalonia;
using Pixed.Common.Platform;

namespace Pixed.Application.Utils;
public static class PlatformUtils
{
    public static IPlatformFolder platformFolder;
    public static AppBuilder SetPlatformFolder(this AppBuilder app, IPlatformFolder folder)
    {
        platformFolder = folder;
        return app;
    }
}
