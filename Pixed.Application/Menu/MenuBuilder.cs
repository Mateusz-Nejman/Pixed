using Pixed.Application.Extensions;
using Pixed.Application.Models;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Utils;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Services;
using Pixed.Common.Utils;
using Pixed.Core;
using Pixed.Core.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;

internal class MenuBuilder(ApplicationData applicationData, IPlatformFolder platformFolder, IHistoryService historyService)
{

    private readonly struct MenuEntry(BaseMenuItem baseMenu, IMenuItem menuItem)
    {
        public BaseMenuItem BaseMenu { get; } = baseMenu;
        public IMenuItem MenuItem { get; } = menuItem;
    }

    private readonly ApplicationData _applicationData = applicationData;
    private readonly IPlatformFolder _platformFolder = platformFolder;
    private readonly IHistoryService _historyService = historyService;
    private readonly List<MenuEntry> _entries = [];

    public Subject<List<IMenuItem>> OnMenuBuilt { get; } = new Subject<List<IMenuItem>>();

    public void AddEntry(BaseMenuItem baseMenu, IMenuItem menuItem)
    {
        _entries.Add(new MenuEntry(baseMenu, menuItem));
    }

    public async Task Build(bool clear = true)
    {
        AddFromExtensions();
        MenuItem baseMenu = new() { Icon = new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_navigation_48_regular.svg") };
        MenuItem fileMenu = new("File");
        MenuItem editMenu = GetEditMenu();
        MenuItem toolsMenu = new("Tools");
        MenuItem paletteMenu = new("Palette");
        MenuItem projectMenu = GetProjectMenu();
        MenuItem viewMenu = new("View");
        MenuItem helpMenu = new("Help");

        MenuItem aboutMenu = new("About")
        {
            Command = ReactiveCommand.CreateFromTask(() => Router.Navigate("/about")),
            Icon = new("avares://Pixed.Application/Resources/icon.png")
        };

        MenuItem onlineHelpMenu = new("Online help")
        {
            Command = new ActionCommand<string>(PlatformUtils.OpenUrl),
            CommandParameter = "https://github.com/Mateusz-Nejman/Pixed/wiki",
            Icon = new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_globe_48_regular.svg")
        };

        helpMenu.Items = [aboutMenu, onlineHelpMenu];

        AddToMenu(ref fileMenu, GetEntries(BaseMenuItem.File));
        AddToMenu(ref toolsMenu, GetEntries(BaseMenuItem.Tools));
        AddToMenu(ref paletteMenu, GetEntries(BaseMenuItem.Palette));
        AddToMenu(ref viewMenu, GetEntries(BaseMenuItem.View));
        AddToMenu(ref helpMenu, GetEntries(BaseMenuItem.Help));

        baseMenu.Items = [fileMenu, editMenu, toolsMenu, paletteMenu, projectMenu, viewMenu, helpMenu];

        List<IMenuItem> menuItems = [baseMenu];
        menuItems.AddRange(GetEntries(BaseMenuItem.Base));

        if (clear)
        {
            _entries.Clear();
        }
        OnMenuBuilt.OnNext(menuItems);
    }

    private MenuItem GetEditMenu()
    {
        MenuItem editMenu = new("Edit");
        AddToMenu(ref editMenu, GetEntries(BaseMenuItem.Edit));

        return editMenu;
    }

    private MenuItem GetProjectMenu()
    {
        MenuItem projectMenu = new("Project");
        MenuItem projectResize = new("Resize project")
        {
            Command = new ActionCommand(async () =>
            {
                var navigatorResult = await Router.Navigate<ResizeResult>("/resizeProject");
                if (navigatorResult.HasValue)
                {
                    var result = navigatorResult.Value;
                    var model = ResizeUtils.ResizeModel(_applicationData, _applicationData.CurrentModel, new Point(result.Width, result.Height), result.ResizeCanvasContent, result.Anchor);
                    _historyService.Register(model);
                    _historyService.CopyHistoryFrom(_applicationData.CurrentModel, model);
                    await _historyService.AddToHistory(model);
                    _applicationData.UserSettings.UserWidth = result.Width;
                    _applicationData.UserSettings.UserHeight = result.Height;
                    _applicationData.UserSettings.MaintainAspectRatio = result.MaintainAspectRatio;
                    await SettingsUtils.Save(_platformFolder, _applicationData);
                    _applicationData.Models[_applicationData.CurrentModelIndex] = model;
                    Subjects.ProjectModified.OnNext(model);
                    Subjects.ProjectChanged.OnNext(model);
                }
            }),
            Icon = new System.Uri("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_resize_48_regular.svg")
        };

        projectMenu.Items = [];

        projectMenu.Items.Add(projectResize);
        AddToMenu(ref projectMenu, GetEntries(BaseMenuItem.Project));
        return projectMenu;
    }

    private List<IMenuItem> GetEntries(BaseMenuItem baseMenu)
    {
        return _entries.Where(e => e.BaseMenu == baseMenu).Select(e => e.MenuItem).ToList();
    }

    private static void AddToMenu(ref MenuItem menuItem, List<IMenuItem> items)
    {
        menuItem.Items ??= [];

        foreach (var item in items)
        {
            menuItem.Items.Add(item);
        }
    }

    private void AddFromExtensions()
    {
        MenuItemRegistry registry = new(this);
        ExtensionsLoader.RegisterMenuItems(registry);
    }
}
