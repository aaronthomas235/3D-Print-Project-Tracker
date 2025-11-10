using _3DPrintProjectTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DPrintProjectTracker.Interfaces
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
