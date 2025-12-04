using Pixed.Application.IO;
using Pixed.Application.Models;
using Pixed.Application.Pages;
using Pixed.Application.Platform;
using Pixed.Application.Routing;
using Pixed.Application.Services;
using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Services;
using Pixed.Common.Services.Keyboard;
using Pixed.Core;
using Pixed.Core.Models;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;
internal class FileMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData, PixedProjectMethods pixedProjectMethods, RecentFilesService recentFilesService, ShortcutService shortcutService, ClipboardService clipboardService, IHistoryService historyService)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly PixedProjectMethods _projectMethods = pixedProjectMethods;
    private readonly RecentFilesService _recentFilesService = recentFilesService;
    private readonly ShortcutService _shortcutService = shortcutService;
    private readonly ClipboardService _clipboardService = clipboardService;
    private readonly IHistoryService _historyService = historyService;

    public async Task Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.File, "New", new AsyncCommand(NewAction), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_document_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, "Create from clipboard", new AsyncCommand(CreateFromClipboardAction), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_document_image_20_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, "Open", new AsyncCommand(OpenAction), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_folder_open_28_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, "Save", new AsyncCommand<bool>(SaveAction), false, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_save_32_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, "Save as", new AsyncCommand<bool>(SaveAction), true, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_save_edit_24_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, "Export to PNG", new AsyncCommand(ExportPngAction), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_image_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, "Export to Ico", new AsyncCommand(ExportIcoAction), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_image_circle_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.File, BuildShareMenu());

        if (IPlatformSettings.Instance.RecentFilesEnabled)
        {
            MenuItem fileRecent = new("Recent")
            {
                Items = await _recentFilesService.BuildMenu()
            };

            _menuItemRegistry.Register(BaseMenuItem.File, fileRecent);
        }

        if (Main.QuitCommand == null)
        {
            return;
        }
        
        _menuItemRegistry.Register(BaseMenuItem.File, "Quit", Main.QuitCommand, null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_exit_20_regular.svg"));

        _shortcutService.Add(KeyState.Control(Avalonia.Input.Key.N), async () => await NewAction());
        _shortcutService.Add(new KeyState(Avalonia.Input.Key.S, true, true, true, false), async () => await SaveAction(true));
        _shortcutService.Add(KeyState.Control(Avalonia.Input.Key.S), async () => await SaveAction(false));
        _shortcutService.Add(KeyState.Control(Avalonia.Input.Key.O), async () => await OpenAction());
    }

    private async Task NewAction()
    {
        var result = await Router.Navigate<NewProjectResult>("/newProject");

        if (result.HasValue)
        {
            PixedModel model = new(result.Value.Width, result.Value.Height)
            {
                FileName = _applicationData.GenerateName()
            };
            _historyService.Register(model);
            _applicationData.Models.Add(model);
            Subjects.ProjectAdded.OnNext(model);
        }
    }

    private async Task CreateFromClipboardAction()
    {
        var png = await _clipboardService.GetPng();
        if (png == null)
        {
            await Router.Message("Opening error", "No image in clipboard");
        }
        else
        {
            await _projectMethods.Create(png);
        }
    }

    private async Task OpenAction()
    {
        await _projectMethods.Open(_recentFilesService);
    }

    private async Task SaveAction(bool saveAs)
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

    private async Task SendProjectAction()
    {
        await Router.Navigate("/sendProject");
    }

    private async Task ReceiveProjectAction()
    {
        await Router.Navigate("/receiveProject");
    }

    private MenuItem BuildShareMenu()
    {
        return new MenuItem("Share")
        {
            Icon = new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_share_48_filled.svg"),
            Items = [BuildShareTcpMenu(), BuildShareBluetoothMenu()]
        };
    }

    private async Task SendBluetoothAction()
    {
        await Router.Navigate("/sendBluetooth");
    }

    private async Task ReceiveBluetoothAction()
    {
        await Router.Navigate("/receiveBluetooth");
    }

    private MenuItem BuildShareTcpMenu()
    {
        MenuItem shareMenu = new("Share") { Command = new AsyncCommand(SendProjectAction) };
        MenuItem receiveMenu = new("Receive") { Command = new AsyncCommand(ReceiveProjectAction) };

        return new MenuItem("Local network")
        {
            Icon = new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_channel_share_48_filled.svg"),
            Items = [shareMenu, receiveMenu]
        };
    }

    private MenuItem BuildShareBluetoothMenu()
    {
        MenuItem shareMenu = new("Share") { Command = new AsyncCommand(SendBluetoothAction) };
        MenuItem receiveMenu = new("Receive") { Command = new AsyncCommand(ReceiveBluetoothAction) };

        return new MenuItem("Bluetooth")
        {
            Icon = new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_bluetooth_48_filled.svg"),
            Items = [shareMenu, receiveMenu]
        };
    }
}