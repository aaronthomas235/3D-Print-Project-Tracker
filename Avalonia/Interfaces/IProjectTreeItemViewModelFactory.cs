using ThreeDPrintProjectTracker.Avalonia.ViewModels;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Avalonia.Interfaces
{
    public interface IProjectTreeItemViewModelFactory
    {
        ProjectTreeItemViewModel Create(ProjectTreeItem model);
    }
}
