using Pixed.Application.Models;
using Pixed.Application.Routing;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Platform;
using Pixed.Core;
using Pixed.Core.Models;

namespace Pixed.Application.Menu;
internal class ViewMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData, IStorageProviderHandle storageProvider)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly IStorageProviderHandle _storageProvider = storageProvider;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle fullscreen", () =>
        {
            //TODO
            /*if (MainWindow.Handle.WindowState == WindowState.FullScreen)
            {
                MainWindow.Handle.WindowState = WindowState.Maximized;
            }
            else
            {
                MainWindow.Handle.WindowState = WindowState.FullScreen;
            }*/
        });

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
        }));

        _menuItemRegistry.Register(BaseMenuItem.View, "Toggle grid", new AsyncCommand(async () =>
        {
            _applicationData.UserSettings.GridEnabled = !_applicationData.UserSettings.GridEnabled;
            _applicationData.UserSettings.Save(await _storageProvider.GetPixedFolder());
            Subjects.GridChanged.OnNext(true);
        }));
    }
}
