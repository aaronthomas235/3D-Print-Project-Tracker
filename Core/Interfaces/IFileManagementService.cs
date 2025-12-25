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

        List<ProjectTreeItemViewModel> BuildProjectDirectoryTree(string projectPath, IProjectTreeItemHost projectTreeItemHost);

        Task<ObservableCollection<ProjectTreeItemViewModel>> LoadProjectsAsync(string projectsRootFolderPath, IProjectTreeItemHost projectTreeItemHost);
        Task SaveProjectsAsync(string projectsRootFolderPath, ObservableCollection<ProjectTreeItemViewModel> projectTreeItems);
    }
}
