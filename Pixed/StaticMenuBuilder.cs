using Avalonia.Controls;
using Pixed.IO;
using Pixed.Models;
using Pixed.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Pixed
{
    internal static class StaticMenuBuilder
    {
        public enum BaseMenuItem
        {
            File,
            Edit,
            Tools,
            Palette,
            Project,
            Help
        }

        private readonly struct MenuEntry(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            public BaseMenuItem BaseMenu { get; } = baseMenu;
            public NativeMenuItem MenuItem { get; } = menuItem;
        }

        private static readonly List<MenuEntry> _entries = [];

        public static Subject<NativeMenu> OnMenuBuilt { get; } = new Subject<NativeMenu>();

        public static void AddEntry(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            _entries.Add(new MenuEntry(baseMenu, menuItem));
        }

        public static void Build(bool clear = true)
        {
            NativeMenuItem fileMenu = GetFileMenu();
            NativeMenuItem editMenu = new("Edit");
            NativeMenuItem toolsMenu = new("Tools");
            NativeMenuItem paletteMenu = new("Palette");
            NativeMenuItem projectMenu = new("Project");
            NativeMenuItem helpMenu = new("Help");

            AddToMenu(ref editMenu, GetEntries(BaseMenuItem.Edit));
            AddToMenu(ref toolsMenu, GetEntries(BaseMenuItem.Tools));
            AddToMenu(ref paletteMenu, GetEntries(BaseMenuItem.Palette));
            AddToMenu(ref projectMenu, GetEntries(BaseMenuItem.Project));
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
            menu.Items.Add(helpMenu);

            OnMenuBuilt.OnNext(menu);
        }

        private static NativeMenuItem GetFileMenu()
        {
            NativeMenuItem fileMenu = new("File");
            NativeMenuItem fileNew = new("New")
            {
                Command = new ActionCommand(async () =>
            {
                NewProjectWindow window = new();
                var success = await window.ShowDialog<bool>(MainWindow.Handle);

                if (success)
                {
                    PixedModel model = new(window.WidthValue, window.HeightValue);
                    Global.Models.Add(model);
                    Subjects.ProjectAdded.OnNext(model);
                }
            })
            };
            NativeMenuItem fileOpen = new("Open")
            {
                Command = new ActionCommand(async () =>
            {
                var files = await IODialogs.OpenFileDialog("All supported (*.pixed;*.png)|*.pixed;*.png|Pixed project (*.pixed)|*.pixed|PNG images (*.png)|*.png", "Open file", true);

                foreach (var item in files)
                {
                    var stream = await item.OpenReadAsync();

                    IPixedProjectSerializer serializer;
                    if (item.Name.EndsWith(".pixed"))
                    {
                        serializer = new PixedProjectSerializer();
                    }
                    else
                    {
                        serializer = new PngProjectSerializer();
                    }

                    PixedModel model = serializer.Deserialize(stream);
                    stream?.Dispose();
                    model.FileName = item.Name.Replace(".png", ".pixed");

                    if (item.Name.EndsWith(".pixed"))
                    {
                        model.FilePath = item.Path.AbsolutePath;
                    }

                    if (Global.CurrentModel.IsEmpty)
                    {
                        Global.Models[Global.CurrentModelIndex] = model;
                    }
                    else
                    {
                        Global.Models.Add(model);
                    }

                    Subjects.ProjectAdded.OnNext(model);
                }
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
            NativeMenuItem fileRecent = new("Recent"); //TODO
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

        private static List<NativeMenuItem> GetEntries(BaseMenuItem baseMenu)
        {
            return _entries.Where(e => e.BaseMenu == baseMenu).Select(e => e.MenuItem).ToList();
        }

        private static void AddToMenu(ref NativeMenuItem menuItem, List<NativeMenuItem> items)
        {
            menuItem.Menu ??= [];

            foreach (var item in items)
            {
                menuItem.Menu.Add(item);
            }
        }

        private async static Task SaveAction(bool saveAs = false)
        {
            Stream fileStream = null;
            if (Global.CurrentModel.FilePath == null)
            {
                saveAs = true;
            }
            else
            {
                fileStream = File.OpenWrite(Global.CurrentModel.FilePath);
            }

            if (saveAs)
            {
                var file = await IODialogs.SaveFileDialog("Pixed project (*.pixed)|*.pixed", Global.CurrentModel.FileName ?? "project.pixed");

                if (file == null)
                {
                    return;
                }

                Global.CurrentModel.FilePath = file.Path.AbsolutePath;
                fileStream = await file.OpenWriteAsync();
            }

            if (fileStream != null)
            {
                PixedProjectSerializer serializer = new PixedProjectSerializer();
                serializer.Serialize(fileStream, Global.CurrentModel, true);
            }
        }

        private async static Task ExportAction()
        {
            var file = await IODialogs.SaveFileDialog("PNG image (*.png)|*.png", Global.CurrentModel.FileName ?? "pixed.png");

            if(file == null)
            {
                return;
            }

            PngProjectSerializer serializer = new PngProjectSerializer();

            int columnsCount = 1;
            if(Global.CurrentModel.Frames.Count > 1)
            {
                ExportPNGWindow window = new();
                bool success = await window.ShowDialog<bool>(MainWindow.Handle);

                if (!success)
                {
                    return;
                }

                columnsCount = window.ColumnsCount;
            }

            serializer.ColumnsCount = columnsCount;
            var stream = await file.OpenWriteAsync();
            serializer.Serialize(stream, Global.CurrentModel, true);
        }
    }
}
