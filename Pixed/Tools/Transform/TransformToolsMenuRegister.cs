using Avalonia.Controls;
using Pixed.Controls;
using Pixed.Windows;
using System;

namespace Pixed.Tools.Transform
{
    internal static class TransformToolsMenuRegister
    {
        public static void Register()
        {
            NativeMenuItem transformMenu = new("Transform")
            {
                Menu = []
            };
            AddToMenu(ref transformMenu, "Flip", () => new TransformFlipWindow());
            AddToMenu(ref transformMenu, "Rotation", () => new TransformRotateWindow());
            AddToMenu(ref transformMenu, "Align image to the center", () => new TransformAlignWindow());
            AddToMenu(ref transformMenu, "Crop to fit the content or the selection", new Crop());

            PixedUserControl.RegisterMenuItem(StaticMenuBuilder.BaseMenuItem.Tools, transformMenu);
        }

        private static void AddToMenu(ref NativeMenuItem menuItem, string text, Func<Window> windowCreator)
        {
            menuItem.Menu ??= [];
            NativeMenuItem toolMenu = new(text)
            {
                Command = new ActionCommand<Func<Window>>(WindowToolAction),
                CommandParameter = windowCreator
            };

            menuItem.Menu.Add(toolMenu);
        }

        private static void AddToMenu(ref NativeMenuItem menuItem, string text, AbstractTransformTool tool)
        {
            menuItem.Menu ??= [];
            NativeMenuItem toolMenu = new(text)
            {
                Command = new ActionCommand<AbstractTransformTool>(ToolAction),
                CommandParameter = tool
            };

            menuItem.Menu.Add(toolMenu);
        }

        private static void WindowToolAction(Func<Window> windowCreator)
        {
            var window = windowCreator();
            window.ShowDialog(MainWindow.Handle);
        }

        private static void ToolAction(AbstractTransformTool tool)
        {
            tool.ApplyTransformation(false, false, false);
        }
    }
}
