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
internal class ViewMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData, IStorageProviderHandle storageProvider, MainWindow mainWindow)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly IStorageProviderHandle _storageProvider = storageProvider;
    private readonly MainWindow _mainWindow = mainWindow;

    public void Register()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _menuItemRegistry.Register(BaseMenuItem.View, "Toggle fullscreen", () =>
            {
                if (_mainWindow.WindowState == WindowState.FullScreen)
                {
                    _mainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    _mainWindow.WindowState = WindowState.FullScreen;
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
