using Avalonia.Controls;
using Pixed.Application.Models;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Core;
using Pixed.Core.Models;
using System.Runtime.InteropServices;

namespace Pixed.Application.Menu;
internal class ViewMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData, IPlatformFolder platformFolder)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly IPlatformFolder _platformFolder = platformFolder;

    public void Register()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _menuItemRegistry.Register(BaseMenuItem.View, "Toggle fullscreen", () =>
            {
                var serviceProvider = App.ServiceProvider;
                var mainWindow = serviceProvider.Get<MainWindow>();
                if (mainWindow.WindowState == WindowState.FullScreen)
                {
                    mainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    mainWindow.WindowState = WindowState.FullScreen;
                }
            }, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_full_screen_maximize_32_regular.svg"));
        }

        _menuItemRegistry.Register(BaseMenuItem.View, "Grid settings", new AsyncCommand(async () =>
        {
            var navigateResult = await Router.Navigate<GridSettingsResult>("/gridSettings");

            if (navigateResult.HasValue)
            {
                _applicationData.UserSettings.GridWidth = navigateResult.Value.Width;
                _applicationData.UserSettings.GridHeight = navigateResult.Value.Height;
                _applicationData.UserSettings.GridColor = navigateResult.Value.Color;
                _applicationData.UserSettings.GridEnabled = true;
                await SettingsUtils.Save(_platformFolder, _applicationData);
                Subjects.GridChanged.OnNext(true);
            }
        }), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_settings_48_regular.svg"));

        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle grid", new AsyncCommand(async () =>
        {
            _applicationData.UserSettings.GridEnabled = !_applicationData.UserSettings.GridEnabled;
            await SettingsUtils.Save(_platformFolder, _applicationData);
            Subjects.GridChanged.OnNext(true);
        }), null, new System.Uri("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_table_simple_48_regular.svg"));

        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle animation preview", async () =>
        {
            bool value = !_applicationData.UserSettings.AnimationPreviewVisible;
            Subjects.AnimationPreviewChanged.OnNext(value);
            _applicationData.UserSettings.AnimationPreviewVisible = value;
            await SettingsUtils.Save(_platformFolder, _applicationData);
        }, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_filmstrip_play_32_regular.svg"));
    }
}
