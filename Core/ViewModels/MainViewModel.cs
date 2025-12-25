using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Core.ViewModels;

public partial class MainViewModel : ObservableObject, IProjectTreeItemHost
{
    public readonly IFileManagementService fileManagementService;
    public readonly IFolderSelectionService folderSelectionService;
    private readonly IThemeChangerService themeChangerService;
    
    public ObservableCollection<ProjectTreeItemViewModel> ProjectTreeItems { get; } = new();

    public IRelayCommand NewProjectTrackerCommand { get; }
    public IAsyncRelayCommand OpenProjectsFolderCommand { get; }
    public IAsyncRelayCommand SaveProjectsCommand { get; }
    public IRelayCommand<string> OpenSelectedPartCommand { get; }

    private bool _isUsingDarkTheme = true;
    public bool IsUsingDarkTheme
    {
        get => _isUsingDarkTheme;
        set
        {
            if (SetProperty(ref _isUsingDarkTheme, value))
            {
                themeChangerService.SetTheme(value);
            }
        }
    }

    private string? _projectsRootFolder;
    public string? ProjectsRootFolder
    {
        get => _projectsRootFolder;
        set => SetProperty(ref _projectsRootFolder, value);
    }

    private ProjectTreeItemViewModel? _ClickedProjectTreeItem;
    public ProjectTreeItemViewModel? ClickedProjectTreeItem
    {
        get => _ClickedProjectTreeItem;
        set
        {
            if (SetProperty(ref _ClickedProjectTreeItem, value))
            {
                OpenSelectedPartCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public MainViewModel(IFileManagementService fileManagementService, IFolderSelectionService folderSelectionService, IThemeChangerService themeChangerService)
    {
        this.fileManagementService = fileManagementService;
        this.folderSelectionService = folderSelectionService;
        this.themeChangerService = themeChangerService;

        NewProjectTrackerCommand = new AsyncRelayCommand(CreateNewProjectTracker);
        OpenProjectsFolderCommand = new AsyncRelayCommand(OpenProjectsFolder);
        SaveProjectsCommand = new AsyncRelayCommand(SaveProjects);
        OpenSelectedPartCommand = new RelayCommand<string>(OpenProjectPartFile, CanOpenFileInSlicer);
    }

    public void OnProjectTreeItemSelected(ProjectTreeItemViewModel item)
    {
        ClickedProjectTreeItem = item;
    }

    private async Task CreateNewProjectTracker()
    {
        ProjectsRootFolder = await folderSelectionService.SelectFolderAsync();
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            return;
        }

        foreach (ProjectTreeItemViewModel projectTreeItem in ProjectTreeItems)
        {
            projectTreeItem.Dispose();
        }
        ProjectTreeItems.Clear();

        var projectsTreeItems = fileManagementService.BuildProjectDirectoryTree(ProjectsRootFolder, this);

        foreach(var projectTreeItem in projectsTreeItems)
        {
            CollapseChildren(projectTreeItem);
            projectTreeItem.IsExpanded = false;
            ProjectTreeItems.Add(projectTreeItem);
        }
    }

    private async Task OpenProjectsFolder()
    {
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            ProjectsRootFolder = await folderSelectionService.SelectFolderAsync();
            if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
            {
                return;
            }
        }

        foreach (var projectTreeItem in ProjectTreeItems)
        {
            projectTreeItem.Dispose();
        }
        ProjectTreeItems.Clear();

        var loadedProjects = await fileManagementService.LoadProjectsAsync(ProjectsRootFolder, this);
        Debug.WriteLine($"Projects: {loadedProjects.Count}");

        if (loadedProjects.Any())
        {
            foreach (var project in loadedProjects)
                ProjectTreeItems.Add(project);

            foreach (var item in ProjectTreeItems)
                item.IsExpanded = true;
        }
        else
        {
            Debug.WriteLine("No existing project data found. Building a new project tree...");
            Debug.WriteLine($"Projects Folder: {ProjectsRootFolder}");
            var projectsTreeItems = fileManagementService.BuildProjectDirectoryTree(ProjectsRootFolder, this);

            foreach (var projectTreeItem in projectsTreeItems)
            {
                CollapseChildren(projectTreeItem);
                projectTreeItem.IsExpanded = false;
                ProjectTreeItems.Add(projectTreeItem);
            }
        }
    }

    private async Task SaveProjects()
    {
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            Debug.WriteLine("No project folder selected.");
            return;
        }

        if (ProjectTreeItems == null || ProjectTreeItems.Count == 0)
        {
            Debug.WriteLine("No projects to save.");
            return;
        }

        try
        {
            await fileManagementService.SaveProjectsAsync(ProjectsRootFolder, ProjectTreeItems);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save projects: {ex.Message}");
        }
    }

    private void CollapseChildren(ProjectTreeItemViewModel parent)
    {
        foreach (var child in parent.Children)
        {
            child.IsExpanded = false;
            CollapseChildren(child);
        }
    }

    private void OpenProjectPartFile(string? partFilePath)
    {
        if (!string.IsNullOrWhiteSpace(partFilePath))
        {
            Debug.WriteLine($"Opening {partFilePath}");
        }
    }

    private bool CanOpenFileInSlicer(string? descriptionParameter)
    {
        if (ClickedProjectTreeItem != null && ClickedProjectTreeItem.IsProjectFile)
        {
            return true;
        }
        return false;
    }
}