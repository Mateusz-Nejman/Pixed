﻿using Avalonia.Controls;
using Pixed.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

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
            NativeMenuItem fileNew = new("New"); //TODO
            NativeMenuItem fileOpen = new("Open"); //TODO
            NativeMenuItem fileSave = new("Save"); //TODO
            NativeMenuItem fileSaveAs = new("Save as"); //TODO
            NativeMenuItem fileRecent = new("Recent"); //TODO
            NativeMenuItem fileQuit = new("Quit");
            fileQuit.Command = MainWindow.QuitCommand;

            fileMenu.Menu = [fileNew, fileOpen, fileSave, fileSaveAs];
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
    }
}