using ThreeDPrintProjectTracker.Engine.Models.Projects;
using ThreeDPrintProjectTracker.Engine.ViewModels;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IProjectTreeItemViewModelFactory
    {
        ProjectTreeItemViewModel Create(ProjectTreeItem model);
    }
}
