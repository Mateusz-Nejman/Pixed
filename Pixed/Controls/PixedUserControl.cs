using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Pixed.StaticMenuBuilder;

namespace Pixed.Controls
{
    internal abstract class PixedUserControl : UserControl
    {
        public virtual void RegisterMenuItems()
        {
            if(DataContext is IPixedViewModel model)
            {
                model.RegisterMenuItems();
            }
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            RegisterMenuItems();
        }

        public static void RegisterMenuItem(BaseMenuItem baseMenu, string text, Action action)
        {
            RegisterMenuItem(baseMenu, text, new ActionCommand(action));
        }

        public static void RegisterMenuItem(BaseMenuItem baseMenu, string text, ICommand command, object? commandParameter = null)
        {
            RegisterMenuItem(baseMenu, new NativeMenuItem(text) { Command = command, CommandParameter = commandParameter });
        }

        public static void RegisterMenuItem(BaseMenuItem baseMenu, NativeMenuItem menuItem)
        {
            StaticMenuBuilder.AddEntry(baseMenu, menuItem);
        }
    }
}
