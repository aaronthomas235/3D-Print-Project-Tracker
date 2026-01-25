using Core.Models;
using Core.ViewModels;

namespace Core.Interfaces
{
    public interface IProjectTreeItemViewModelFactory
    {
        ProjectTreeItemViewModel Create(ProjectTreeItem model);
    }
}
