using ThreeDPrintProjectTracker.Engine.Models;
using ThreeDPrintProjectTracker.Engine.ViewModels;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IProjectTreeItemViewModelFactory
    {
        ProjectTreeItemViewModel Create(ProjectTreeItem model);
    }
}
