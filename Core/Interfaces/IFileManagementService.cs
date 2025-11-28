using Core.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileManagementService
    {
        string[] GetProjectDirectories(string ProjectDirectoriesFilePath);
        string[] GetProjectFiles(string ProjectFilesFilePath);

        List<ExpanderItemViewModel> BuildProjectDirectoryTree(string projectPath, IExpanderItemHost expanderItemHost);

        Task<ObservableCollection<ExpanderItemViewModel>> LoadProjectsAsync(string projectsRootFolderPath, IExpanderItemHost expanderItemHost);
        Task SaveProjectsAsync(string projectsRootFolderPath, ObservableCollection<ExpanderItemViewModel> expanderItems);
    }
}
