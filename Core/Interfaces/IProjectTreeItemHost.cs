using Core.ViewModels;

namespace Core.Interfaces
{
    public interface IProjectTreeItemHost
    {
        void OnProjectTreeItemSelected(ProjectTreeItemViewModel item);
    }
}
