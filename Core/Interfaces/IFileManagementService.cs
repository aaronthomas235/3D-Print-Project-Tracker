using Core.Services;
using Core.ViewModels;
using Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileManagementService
    {
        Task<IReadOnlyList<ProjectTreeItem>> LoadProjectModelsAsync(string rootFolderPath);
        Task SaveProjectsAsync(string rootFolder, IReadOnlyList<ProjectTreeItem> items);
    }
}