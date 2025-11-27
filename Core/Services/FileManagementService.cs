using Core.Interfaces;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Services
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

        public List<ExpanderItemViewModel> BuildProjectDirectoryTree(string projectPath, IExpanderItemHost expanderItemHost)
        {
            var items = new List<ExpanderItemViewModel>();

            var directories = GetProjectDirectories(projectPath);
            var files = GetProjectFiles(projectPath);

            /*string projectName = Path.GetFileName(projectPath);
            

            var expanderItemNode = new ExpanderItemViewModel(expanderItemHost)
            {
                Title = projectName,
                Description = projectPath,
                IsChecked = false,
                IsProjectFile = false
            };*/

            foreach(var directory in directories)
            {
                var directoryNode = new ExpanderItemViewModel(expanderItemHost)
                {
                    Title = Path.GetFileName(directory),
                    Description = directory,
                    IsChecked = false,
                    IsProjectFile = false
                };

                var childItems = BuildProjectDirectoryTree(directory, expanderItemHost);
                foreach(var childItem in childItems)
                {
                    directoryNode.Children.Add(childItem);
                }
                items.Add(directoryNode);
            }

            foreach(var file in files)
            {
                items.Add(new ExpanderItemViewModel()
                {
                    Title = Path.GetFileName(file),
                    Description = file,
                    IsChecked = false,
                    IsProjectFile = true
                });
            }

            return items;
        }

        public async Task<ObservableCollection<ExpanderItemViewModel>> LoadProjectsAsync(string projectRootFolderPath, IExpanderItemHost expanderItemHost)
        {
            if (string.IsNullOrWhiteSpace(projectRootFolderPath) || !Directory.Exists(projectRootFolderPath))
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
                string json = await File.ReadAllTextAsync(saveFilePath);
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var projects = JsonSerializer.Deserialize<ObservableCollection<ExpanderItemViewModel>>(json, options) ?? new ObservableCollection<ExpanderItemViewModel>();
                Debug.WriteLine($"Project Child Count: {projects[0].Children.Count}");
                foreach(var project in projects)
                {
                    project.InitialiseRuntimeResources(expanderItemHost);
                }
                return projects;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load projects: {ex.Message}");
                return new ObservableCollection<ExpanderItemViewModel>();
            }
        }

        public async Task SaveProjectsAsync(string projectsRootFolderPath, ObservableCollection<ExpanderItemViewModel> expanderItems)
        {
            if (string.IsNullOrWhiteSpace(projectsRootFolderPath) || expanderItems == null)
                return;

            try
            {
                Directory.CreateDirectory(projectsRootFolderPath);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string json = JsonSerializer.Serialize(expanderItems, options);
                string savePath = Path.Combine(projectsRootFolderPath, "projectSaveData.json");

                await File.WriteAllTextAsync(savePath, json);
                Debug.WriteLine($"Projects successfully saved: {savePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving projects: {ex.Message}");
                throw; // Let ViewModel handle the UI notification
            }
        }
    }
}
