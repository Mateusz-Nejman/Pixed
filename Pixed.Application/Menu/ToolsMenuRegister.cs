using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Pixed.Common.Menu;
using Pixed.Common.Tools;
using Pixed.Core;
using System;
using System.Collections.Generic;

namespace Pixed.Application.Menu;
internal class ToolsMenuRegister(IMenuItemRegistry menuItemRegistry, ToolSelector toolSelector)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ToolSelector _toolSelector = toolSelector;

    public void Register()
    {

        foreach(var tool in _toolSelector.GetTools())
        {
            RegisterTool(tool.Value);
        }
    }

    private void RegisterTool(BaseTool tool)
    {
        ActionCommand<string> selectToolAction = new(_toolSelector.SelectTool);
        Uri uri = new(tool.ImagePath);
        Uri? icon = null;
        if (AssetLoader.Exists(uri))
        {
            icon = uri;
        }

        _menuItemRegistry.Register(BaseMenuItem.Tools, tool.Name, selectToolAction, tool.Id, icon);
    }
}
