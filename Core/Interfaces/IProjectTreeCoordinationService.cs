using ThreeDPrintProjectTracker.Engine.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IProjectTreeCoordinationService
    {
        Task<IReadOnlyList<ProjectTreeItemViewModel>> LoadProjectsAsync(string rootFolder);
        Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItemViewModel> items);
    }
}
