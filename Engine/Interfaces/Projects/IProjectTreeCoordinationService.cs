using System.Collections.Generic;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IProjectTreeCoordinationService
    {
        Task<IReadOnlyList<ProjectTreeItem>> LoadProjectsAsync(string rootFolder);
        Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItem> items);
    }
}
