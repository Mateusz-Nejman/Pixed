using Avalonia.Controls;
using Pixed.IO;
using Pixed.Models;
using Pixed.Services;
using Pixed.Utils;
using Pixed.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Pixed.Menu;

internal class MenuBuilder(ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, RecentFilesService recentFilesService)
{
    public enum BaseMenuItem
    {
        File,
        Edit,
        Tools,
        Palette,
        Project,
        View,
        Help
    }

    private readonly struct MenuEntry(BaseMenuItem baseMenu, NativeMenuItem menuItem)
    {
        public BaseMenuItem BaseMenu { get; } = baseMenu;
        public NativeMenuItem MenuItem { get; } = menuItem;
    }

    private readonly ApplicationData _applicationData = applicationData;
    private readonly PixedProjectMethods _projectMethods = pixedProjectMethods;
    private readonly RecentFilesService _recentFilesService = recentFilesService;
    private readonly List<MenuEntry> _entries = [];

    public Subject<NativeMenu> OnMenuBuilt { get; } = new Subject<NativeMenu>();

    public void AddEntry(BaseMenuItem baseMenu, NativeMenuItem menuItem)
    {
        _entries.Add(new MenuEntry(baseMenu, menuItem));
    }

    public void Build(bool clear = true)
    {
        NativeMenuItem fileMenu = GetFileMenu();
        NativeMenuItem editMenu = GetEditMenu();
        NativeMenuItem toolsMenu = new("Tools");
        NativeMenuItem paletteMenu = new("Palette");
        NativeMenuItem projectMenu = GetProjectMenu();
        NativeMenuItem viewMenu = new("View");
        NativeMenuItem helpMenu = new("Help");

        NativeMenuItem aboutMenu = new("About")
        {
            Command = new ActionCommand(() =>
        {
            AboutWindow window = new();
            window.ShowDialog(MainWindow.Handle);
        })
        };

        helpMenu.Menu = [aboutMenu];

        AddToMenu(ref toolsMenu, GetEntries(BaseMenuItem.Tools));
        AddToMenu(ref paletteMenu, GetEntries(BaseMenuItem.Palette));
        AddToMenu(ref viewMenu, GetEntries(BaseMenuItem.View));
        AddToMenu(ref helpMenu, GetEntries(BaseMenuItem.Help));

        if (clear)
        {
            _entries.Clear();
        }

        NativeMenu menu = [];
        menu.Items.Add(fileMenu);
        menu.Items.Add(editMenu);
        menu.Items.Add(toolsMenu);
        menu.Items.Add(paletteMenu);
        menu.Items.Add(projectMenu);
        menu.Items.Add(viewMenu);
        menu.Items.Add(helpMenu);

        OnMenuBuilt.OnNext(menu);
    }

    private NativeMenuItem GetFileMenu()
    {
        NativeMenuItem fileMenu = new("File");
        NativeMenuItem fileNew = new("New")
        {
            Command = new ActionCommand(async () =>
        {
            NewProjectWindow window = new(_applicationData);
            var success = await window.ShowDialog<bool>(MainWindow.Handle);

            if (success)
            {
                PixedModel model = new(_applicationData, window.WidthValue, window.HeightValue)
                {
                    FileName = _applicationData.GenerateName()
                };
                _applicationData.Models.Add(model);
                Subjects.ProjectAdded.OnNext(model);
            }
        })
        };
        NativeMenuItem fileOpen = new("Open")
        {
            Command = new ActionCommand(async () =>
        {
            await _projectMethods.Open(_recentFilesService);
        })
        };
        NativeMenuItem fileSave = new("Save")
        {
            Command = new AsyncCommand<bool>(SaveAction),
            CommandParameter = false
        };
        NativeMenuItem fileSaveAs = new("Save as")
        {
            Command = new AsyncCommand<bool>(SaveAction),
            CommandParameter = true
        };
        NativeMenuItem fileExport = new("Export to PNG")
        {
            Command = new AsyncCommand(ExportAction)
        };
        NativeMenuItem fileRecent = new("Recent")
        {
            Menu = _recentFilesService.BuildMenu()
        };

        NativeMenuItem fileQuit = new("Quit")
        {
            Command = MainWindow.QuitCommand
        };

        fileMenu.Menu = [fileNew, fileOpen, fileSave, fileSaveAs, fileExport];
        AddToMenu(ref fileMenu, GetEntries(BaseMenuItem.File));

        fileMenu.Menu.Add(fileRecent);
        fileMenu.Menu.Add(fileQuit);
        return fileMenu;
    }

    private NativeMenuItem GetEditMenu()
    {
        NativeMenuItem editMenu = new("Edit");
        NativeMenuItem undoMenu = new("Undo")
        {
            Command = new ActionCommand(() =>
            {
                _applicationData.CurrentModel.Undo();
                Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
            })
        };
        NativeMenuItem redoMenu = new("Redo")
        {
            Command = new ActionCommand(() =>
            {
                _applicationData.CurrentModel.Redo();
                Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
            })
        };
        
        editMenu.Menu = [undoMenu, redoMenu];
        AddToMenu(ref editMenu, GetEntries(BaseMenuItem.Edit));

        return editMenu;
    }

    private NativeMenuItem GetProjectMenu()
    {
        NativeMenuItem projectMenu = new("Project");
        NativeMenuItem projectResize = new("Resize project");

        projectMenu.Menu = [];
        projectResize.Command = new ActionCommand(async () =>
        {
            ResizeProjectWindow window = new(_applicationData.CurrentModel);
            bool success = await window.ShowDialog<bool>(MainWindow.Handle);

            if (success)
            {
                var result = window.Result;
                var model = ResizeUtils.ResizeModel(_applicationData, _applicationData.CurrentModel, result.Width, result.Height, result.ResizeCanvasContent, result.Anchor);
                _applicationData.Models[_applicationData.CurrentModelIndex] = model;
                Subjects.ProjectModified.OnNext(model);
                Subjects.ProjectChanged.OnNext(model);
            }
        });

        projectMenu.Menu.Add(projectResize);
        AddToMenu(ref projectMenu, GetEntries(BaseMenuItem.Project));
        return projectMenu;
    }

    private List<NativeMenuItem> GetEntries(BaseMenuItem baseMenu)
    {
        return _entries.Where(e => e.BaseMenu == baseMenu).Select(e => e.MenuItem).ToList();
    }

    private void AddToMenu(ref NativeMenuItem menuItem, List<NativeMenuItem> items)
    {
        menuItem.Menu ??= [];

        foreach (var item in items)
        {
            menuItem.Menu.Add(item);
        }
    }

    private async Task SaveAction(bool saveAs = false)
    {
        await _projectMethods.Save(_applicationData.CurrentModel, saveAs, _recentFilesService);
    }

    private async Task ExportAction()
    {
        await _projectMethods.ExportToPng(_applicationData.CurrentModel);
    }
}
