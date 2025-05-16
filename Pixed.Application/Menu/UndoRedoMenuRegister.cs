using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Models;
using Pixed.Common.Services;
using Pixed.Core;
using Pixed.Core.Models;
using System.Threading.Tasks;

namespace Pixed.Application.Menu;
internal class UndoRedoMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData, IHistoryService historyService)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;
    private readonly IHistoryService _historyService = historyService;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Undo", new AsyncCommand(Undo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_undo_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Redo", new AsyncCommand(Redo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_redo_48_regular.svg"));

        _menuItemRegistry.Register(BaseMenuItem.Base, "Undo", new AsyncCommand(Undo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_undo_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Base, "Redo", new AsyncCommand(Redo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_redo_48_regular.svg"));
    }

    private async Task Undo()
    {
        await _applicationData.CurrentModel.Undo(_historyService);
        Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
    }

    private async Task Redo()
    {
        await _applicationData.CurrentModel.Redo(_historyService);
        Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
    }
}
