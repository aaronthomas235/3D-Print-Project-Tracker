using Core.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileManagementService
    {
        string[] GetProjectDirectories(string ProjectDirectoriesFilePath);
        string[] GetProjectFiles(string ProjectFilesFilePath);

        ExpanderItemViewModel BuildProjectDirectoryTree(string projectPath, IExpanderItemHost expanderItemHost);

        Task<ObservableCollection<ExpanderItemViewModel>> LoadProjectsAsync(string projectsRootFolderPath, IExpanderItemHost expanderItemHost);
        Task SaveProjectsAsync(string projectsRootFolderPath, ObservableCollection<ExpanderItemViewModel> expanderItems);
    }
}
