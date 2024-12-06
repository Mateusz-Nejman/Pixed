using Avalonia.Controls;
using Pixed.Application.Extensions;
using Pixed.Application.IO;
using Pixed.Application.Models;
using Pixed.Application.Routing;
using Pixed.Application.Services;
using Pixed.Application.Windows;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Models;
using Pixed.Common.Platform;
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

internal class MenuBuilder(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, RecentFilesService recentFilesService, IStorageProviderHandle storageProvider)
{

    private readonly struct MenuEntry(BaseMenuItem baseMenu, IMenuItem menuItem)
    {
        public BaseMenuItem BaseMenu { get; } = baseMenu;
        public IMenuItem MenuItem { get; } = menuItem;
    }

    private readonly ApplicationData _applicationData = applicationData;
    private readonly PixedProjectMethods _projectMethods = pixedProjectMethods;
    private readonly RecentFilesService _recentFilesService = recentFilesService;
    private readonly IStorageProviderHandle _storageProvider = storageProvider;
    private readonly List<MenuEntry> _entries = [];

    public Subject<List<IMenuItem>> OnMenuBuilt { get; } = new Subject<List<IMenuItem>>();

    public void AddEntry(BaseMenuItem baseMenu, IMenuItem menuItem)
    {
        _entries.Add(new MenuEntry(baseMenu, menuItem));
    }

    public void Build(bool clear = true)
    {
        AddFromExtensions();
        MenuItem fileMenu = GetFileMenu();
        MenuItem editMenu = GetEditMenu();
        MenuItem toolsMenu = new("Tools");
        MenuItem paletteMenu = new("Palette");
        MenuItem projectMenu = GetProjectMenu();
        MenuItem viewMenu = new("View");
        MenuItem helpMenu = new("Help");

        MenuItem aboutMenu = new("About")
        {
            Command = ReactiveCommand.CreateFromTask(() => Router.Navigate("/about"))
        };

        helpMenu.Items = [aboutMenu];

        AddToMenu(ref toolsMenu, GetEntries(BaseMenuItem.Tools));
        AddToMenu(ref paletteMenu, GetEntries(BaseMenuItem.Palette));
        AddToMenu(ref viewMenu, GetEntries(BaseMenuItem.View));
        AddToMenu(ref helpMenu, GetEntries(BaseMenuItem.Help));

        if (clear)
        {
            _entries.Clear();
        }

        NativeMenu menu = [];

        List<IMenuItem> items = [fileMenu, editMenu, toolsMenu, paletteMenu, projectMenu, viewMenu, helpMenu];
        OnMenuBuilt.OnNext(items);
    }

    private MenuItem GetFileMenu()
    {
        MenuItem fileMenu = new("File");
        MenuItem fileNew = new("New")
        {
            Command = new ActionCommand(async () =>
        {
            var result = await Router.Navigate<NewProjectResult>("/newProject");

            if (result.HasValue)
            {
                PixedModel model = new(_applicationData, result.Value.Width, result.Value.Height)
                {
                    FileName = _applicationData.GenerateName()
                };
                _applicationData.Models.Add(model);
                Subjects.ProjectAdded.OnNext(model);
            }
        })
        };
        MenuItem fileOpen = new("Open")
        {
            Command = new ActionCommand(async () =>
        {
            await _projectMethods.Open(_recentFilesService);
        })
        };
        MenuItem fileSave = new("Save")
        {
            Command = new AsyncCommand<bool>(SaveAction),
            CommandParameter = false
        };
        MenuItem fileSaveAs = new("Save as")
        {
            Command = new AsyncCommand<bool>(SaveAction),
            CommandParameter = true
        };
        MenuItem fileExportPng = new("Export to PNG")
        {
            Command = new AsyncCommand(ExportPngAction)
        };
        MenuItem fileExportIco = new("Export to Ico")
        {
            Command = new AsyncCommand(ExportIcoAction)
        };
        MenuItem fileRecent = new("Recent")
        {
            Items = _recentFilesService.BuildMenu()
        };

        MenuItem fileQuit = new("Quit")
        {
            Command = MainView.QuitCommand
        };

        fileMenu.Items = [fileNew, fileOpen, fileSave, fileSaveAs, fileExportPng, fileExportIco];
        AddToMenu(ref fileMenu, GetEntries(BaseMenuItem.File));

        fileMenu.Items.Add(fileRecent);
        fileMenu.Items.Add(fileQuit);
        return fileMenu;
    }

    private MenuItem GetEditMenu()
    {
        MenuItem editMenu = new("Edit");
        MenuItem undoMenu = new("Undo")
        {
            Command = new ActionCommand(() =>
            {
                _applicationData.CurrentModel.Undo();
                Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
            })
        };
        MenuItem redoMenu = new("Redo")
        {
            Command = new ActionCommand(() =>
            {
                _applicationData.CurrentModel.Redo();
                Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
            })
        };

        editMenu.Items = [undoMenu, redoMenu];
        AddToMenu(ref editMenu, GetEntries(BaseMenuItem.Edit));

        return editMenu;
    }

    private MenuItem GetProjectMenu()
    {
        MenuItem projectMenu = new("Project");
        MenuItem projectResize = new("Resize project");

        projectMenu.Items = [];
        projectResize.Command = new ActionCommand(async () =>
        {
            var navigatorResult = await Router.Navigate<ResizeResult>("/resizeProject");
            if (navigatorResult.HasValue)
            {
                var result = navigatorResult.Value;
                var model = ResizeUtils.ResizeModel(_applicationData, _applicationData.CurrentModel, new Point(result.Width, result.Height), result.ResizeCanvasContent, result.Anchor);
                _applicationData.UserSettings.UserWidth = result.Width;
                _applicationData.UserSettings.UserHeight = result.Height;
                _applicationData.UserSettings.MaintainAspectRatio = result.MaintainAspectRatio;
                _applicationData.UserSettings.Save(await _storageProvider.GetPixedFolder());
                _applicationData.Models[_applicationData.CurrentModelIndex] = model;
                Subjects.ProjectModified.OnNext(model);
                Subjects.ProjectChanged.OnNext(model);
            }
        });

        projectMenu.Items.Add(projectResize);
        AddToMenu(ref projectMenu, GetEntries(BaseMenuItem.Project));
        return projectMenu;
    }

    private List<IMenuItem> GetEntries(BaseMenuItem baseMenu)
    {
        return _entries.Where(e => e.BaseMenu == baseMenu).Select(e => e.MenuItem).ToList();
    }

    private void AddToMenu(ref MenuItem menuItem, List<IMenuItem> items)
    {
        menuItem.Items ??= [];

        foreach (var item in items)
        {
            menuItem.Items.Add(item);
        }
    }

    private async Task SaveAction(bool saveAs = false)
    {
        await _projectMethods.Save(_applicationData.CurrentModel, saveAs, _recentFilesService);
    }

    private async Task ExportPngAction()
    {
        await _projectMethods.ExportToPng(_applicationData.CurrentModel);
    }

    private async Task ExportIcoAction()
    {
        await _projectMethods.ExportToIco(_applicationData.CurrentModel);
    }

    private void AddFromExtensions()
    {
        MenuItemRegistry registry = new(this);
        ExtensionsLoader.RegisterMenuItems(registry);
    }
}
