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
        private readonly ISupportedFileFormatsService _supportedFileFormatsService;
        public FileManagementService(ISupportedFileFormatsService supportedFileFormatsService) {
            _supportedFileFormatsService = supportedFileFormatsService;

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
                .Where(file => _supportedFileFormatsService.IsExtensionSupported(Path.GetExtension(file)))
                .ToArray();

            return projectFiles;
        }

        public List<ProjectTreeItemViewModel> BuildProjectDirectoryTree(string projectPath, IProjectTreeItemHost projectTreeItemHost, IMeshAnalyserService meshAnalyserService)
        {
            var items = new List<ProjectTreeItemViewModel>();

            var directories = GetProjectDirectories(projectPath);
            var files = GetProjectFiles(projectPath);

            foreach(var directory in directories)
            {
                var directoryNode = new ProjectTreeItemViewModel(projectTreeItemHost, meshAnalyserService)
                {
                    Title = Path.GetFileName(directory),
                    Description = directory,
                    IsChecked = false,
                    IsProjectFile = false,
                    PartName = String.Empty,
                    Dimensions = String.Empty,
                    PrintTime = String.Empty,
                    MaterialUsage = String.Empty
                };

                var childItems = BuildProjectDirectoryTree(directory, projectTreeItemHost, meshAnalyserService);
                foreach(var childItem in childItems)
                {
                    directoryNode.Children.Add(childItem);
                }
                items.Add(directoryNode);
            }

            foreach(var file in files)
            {
                items.Add(new ProjectTreeItemViewModel(projectTreeItemHost, meshAnalyserService)
                {
                    Title = Path.GetFileName(file),
                    Description = file,
                    IsChecked = false,
                    IsProjectFile = true,
                    PartName = Path.GetFileNameWithoutExtension(file),
                    Dimensions = "- x - x -",
                    PrintTime = "0 Hrs 0 Min",
                    MaterialUsage = "0g"
                });
            }

            return items;
        }

        public async Task<ObservableCollection<ProjectTreeItemViewModel>> LoadProjectsAsync(string projectRootFolderPath, IProjectTreeItemHost projectTreeItemHost)
        {
            if (string.IsNullOrWhiteSpace(projectRootFolderPath) || !Directory.Exists(projectRootFolderPath))
            {
                return new ObservableCollection<ProjectTreeItemViewModel>();
            }

            string saveFilePath = Path.Combine(projectRootFolderPath, "projectSaveData.json");

            if (!File.Exists(saveFilePath))
            {
                return new ObservableCollection<ProjectTreeItemViewModel>();
            }
            try
            {
                string json = await File.ReadAllTextAsync(saveFilePath);
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var projects = JsonSerializer.Deserialize<ObservableCollection<ProjectTreeItemViewModel>>(json, options) ?? new ObservableCollection<ProjectTreeItemViewModel>();
                Debug.WriteLine($"Project Child Count: {projects[0].Children.Count}");
                foreach(var project in projects)
                {
                    project.InitialiseRuntimeResources(projectTreeItemHost);
                }
                return projects;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load projects: {ex.Message}");
                return new ObservableCollection<ProjectTreeItemViewModel>();
            }
        }

        public async Task SaveProjectsAsync(string projectsRootFolderPath, ObservableCollection<ProjectTreeItemViewModel> projectTreeItems)
        {
            if (string.IsNullOrWhiteSpace(projectsRootFolderPath) || projectTreeItems == null)
                return;

            try
            {
                Directory.CreateDirectory(projectsRootFolderPath);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string json = JsonSerializer.Serialize(projectTreeItems, options);
                string savePath = Path.Combine(projectsRootFolderPath, "projectSaveData.json");

                await File.WriteAllTextAsync(savePath, json);
                Debug.WriteLine($"Projects successfully saved: {savePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving projects: {ex.Message}");
                throw;
            }
        }
    }
}