using Avalonia;
using Pixed.Application.IO;
using Pixed.Application.Platform;

namespace Pixed.Application.Utils;
public static class PlatformUtils
{
    public static IPlatformFolder platformFolder = new DefaultPlatformFolder();
    public static AppBuilder SetPlatformFolder(this AppBuilder app, IPlatformFolder folder)
    {
        platformFolder = folder;
        return app;
    }
}
