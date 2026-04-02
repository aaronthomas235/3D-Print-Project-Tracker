using System.Collections.Generic;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IFileManagementService
    {
        Task<IReadOnlyList<ProjectTreeItem>> LoadProjectModelsAsync(string rootFolderPath);
        Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItem> items);
    }
}