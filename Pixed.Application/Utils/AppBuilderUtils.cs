using Avalonia;
using AvaloniaInside.Shell;
using ReactiveUI.Avalonia;

namespace Pixed.Application.Utils;

public static class AppBuilderUtils
{
    public static AppBuilder GetDefault(this AppBuilder builder)
    {
        return builder.WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .UseShell()
            .With(new SkiaOptions() { MaxGpuResourceSizeBytes = long.MaxValue });
    }
}
