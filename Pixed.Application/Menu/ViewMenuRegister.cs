using Avalonia.Controls;
using Pixed.Application.Models;
using Pixed.Application.Routing;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Platform;
using Pixed.Core;
using Pixed.Core.Models;
using System.Runtime.InteropServices;

namespace Pixed.Application.Menu;
internal class ViewMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData, IStorageProviderHandle storageProvider)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly IStorageProviderHandle _storageProvider = storageProvider;

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
            }, new("avares://Pixed.Application/Resources/Icons/enlarge-menu.png"));
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
                _applicationData.UserSettings.Save(await _storageProvider.GetPixedFolder());
                Subjects.GridChanged.OnNext(true);
            }
        }), null, new("avares://Pixed.Application/Resources/Icons/cogs-menu.png"));

        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle grid", new AsyncCommand(async () =>
        {
            _applicationData.UserSettings.GridEnabled = !_applicationData.UserSettings.GridEnabled;
            _applicationData.UserSettings.Save(await _storageProvider.GetPixedFolder());
            Subjects.GridChanged.OnNext(true);
        }));
    }
}
