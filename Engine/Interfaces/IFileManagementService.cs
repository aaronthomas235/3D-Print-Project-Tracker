using ThreeDPrintProjectTracker.Engine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IFileManagementService
    {
        Task<IReadOnlyList<ProjectTreeItem>> LoadProjectModelsAsync(string rootFolderPath);
        Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItem> items);
    }
}