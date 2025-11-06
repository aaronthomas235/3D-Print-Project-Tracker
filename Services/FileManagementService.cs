using _3DPrintProjectTracker.Interfaces;
using _3DPrintProjectTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _3DPrintProjectTracker.Services
{
    public class FileManagementService : IFileManagementService
    {
        private readonly HashSet<string> _supportedFileExtensionsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".stl", ".obj", ".3mf", ".dae", ".ply", ".gltf", ".glb", ".x3d", ".amf"
        };
        public FileManagementService() {
        }

        public string[] GetProjectDirectories(string projectDirectoriesFilePath)
        {
            if (string.IsNullOrWhiteSpace(projectDirectoriesFilePath) || !Directory.Exists(projectDirectoriesFilePath))
            {
                return Array.Empty<string>();
            }
            return Directory.GetDirectories(projectDirectoriesFilePath, "*", SearchOption.TopDirectoryOnly);
        }

        public string[] GetProjectFiles(string projectFilesFilePath)
        {
            string[] projectFiles;
            if (string.IsNullOrWhiteSpace(projectFilesFilePath) || !Directory.Exists(projectFilesFilePath))
            {
                return Array.Empty<string>();
            }

            projectFiles = Directory.GetFiles(projectFilesFilePath)
                .Where(file => _supportedFileExtensionsSet.Contains(Path.GetExtension(file)))
                .ToArray();

            return projectFiles;
        }

        public ExpanderItemViewModel BuildProjectDirectoryTree(string projectPath, MainViewModel parent)
        {
            string projectName = Path.GetFileName(projectPath);
            var projectDirectories = GetProjectDirectories(projectPath);
            var projectFiles = GetProjectFiles(projectPath);

            var expanderItemNode = new ExpanderItemViewModel(parent)
            {
                Title = projectName,
                Description = projectPath,
                IsChecked = false,
                IsProjectFile = false
            };

            foreach(var directory in projectDirectories)
            {
                expanderItemNode.Children.Add(BuildProjectDirectoryTree(directory, parent));
            }

            foreach(var file in projectFiles)
            {
                expanderItemNode.Children.Add(new ExpanderItemViewModel(parent)
                {
                    Title = Path.GetFileName(file),
                    Description = file,
                    IsChecked = false,
                    IsProjectFile = true
                });
            }

            return expanderItemNode;
        }

        public async Task<ObservableCollection<ExpanderItemViewModel>> LoadProjectsAsync(string projectRootFolderPath)
        {
            if (!string.IsNullOrWhiteSpace(projectRootFolderPath) || !Directory.Exists(projectRootFolderPath))
            {
                return new ObservableCollection<ExpanderItemViewModel>();
            }

            string saveFilePath = Path.Combine(projectRootFolderPath, "projectSaveData.json");

            if (!File.Exists(saveFilePath))
            {
                return new ObservableCollection<ExpanderItemViewModel>();
            }

            try
            {
                string projectsJson = await File.ReadAllTextAsync(saveFilePath);
                var deserialisationOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                return JsonSerializer.Deserialize<ObservableCollection<ExpanderItemViewModel>>(projectsJson, deserialisationOptions) ?? new ObservableCollection<ExpanderItemViewModel>();
            }
            catch(Exception exception)
            {
                Debug.WriteLine($"Failed to load projects: {exception.Message}");
                return new ObservableCollection<ExpanderItemViewModel>();
            }
        }

        public async Task SaveProjectsAsync(string projectsRootFolderPath, ObservableCollection<ExpanderItemViewModel> expanderItems)
        {
            if (!string.IsNullOrWhiteSpace(projectsRootFolderPath) || expanderItems == null)
            {
                return;
            }

            try
            {
                var serialisationOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string projectsJson = JsonSerializer.Serialize(expanderItems, serialisationOptions);
                string projectsSaveFilePath = Path.Combine(projectsRootFolderPath, "projectsSaveData.json");
                await File.WriteAllTextAsync(projectsSaveFilePath, projectsJson);
                Debug.WriteLine($"Projects successfully saved: {projectsSaveFilePath}");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"failed to save projects: {exception.Message}");
            }

        }
    }
}
