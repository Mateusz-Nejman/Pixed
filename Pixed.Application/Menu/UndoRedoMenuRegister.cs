using Pixed.Common;
using Pixed.Common.Menu;
using Pixed.Common.Models;
using Pixed.Core;
using Pixed.Core.Models;

namespace Pixed.Application.Menu;
internal class UndoRedoMenuRegister(IMenuItemRegistry menuItemRegistry, ApplicationData applicationData)
{
    private readonly IMenuItemRegistry _menuItemRegistry = menuItemRegistry;
    private readonly ApplicationData _applicationData = applicationData;

    public void Register()
    {
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Undo", new ActionCommand(Undo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_undo_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Edit, "Redo", new ActionCommand(Redo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_redo_48_regular.svg"));

        _menuItemRegistry.Register(BaseMenuItem.Base, "Undo", new ActionCommand(Undo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_undo_48_regular.svg"));
        _menuItemRegistry.Register(BaseMenuItem.Base, "Redo", new ActionCommand(Redo), null, new("avares://Pixed.Application/Resources/fluent-icons/ic_fluent_arrow_redo_48_regular.svg"));
    }

    private void Undo()
    {
        _applicationData.CurrentModel.Undo();
        Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
    }

    private void Redo()
    {
        _applicationData.CurrentModel.Redo();
        Subjects.ProjectModified.OnNext(_applicationData.CurrentModel);
    }
}
