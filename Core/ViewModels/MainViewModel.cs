using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core.ViewModels;

public partial class MainViewModel : ObservableObject, IExpanderItemHost
{
    public readonly IFileManagementService fileManagementService;
    public readonly IFolderSelectionService folderSelectionService;
    public ObservableCollection<ExpanderItemViewModel> ExpanderItems { get; } = new();

    public IRelayCommand NewProjectTrackerCommand { get; }
    public IAsyncRelayCommand OpenProjectsFolderCommand { get; }
    public IAsyncRelayCommand SaveProjectsCommand { get; }
    public IRelayCommand<string> OpenSelectedPartCommand { get; }

    private string? _projectsRootFolder;
    public string? ProjectsRootFolder
    {
        get => _projectsRootFolder;
        set => SetProperty(ref _projectsRootFolder, value);
    }

    private ExpanderItemViewModel _clickedExpanderItem;
    public ExpanderItemViewModel ClickedExpanderItem
    {
        get => _clickedExpanderItem;
        set
        {
            if (SetProperty(ref _clickedExpanderItem, value))
            {
                OpenSelectedPartCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public MainViewModel(IFileManagementService fileManagementService, IFolderSelectionService folderSelectionService)
    {
        this.fileManagementService = fileManagementService;
        this.folderSelectionService = folderSelectionService;

        NewProjectTrackerCommand = new AsyncRelayCommand(CreateNewProjectTracker);
        OpenProjectsFolderCommand = new AsyncRelayCommand(OpenProjectsFolder);
        SaveProjectsCommand = new AsyncRelayCommand(SaveProjects);
        OpenSelectedPartCommand = new RelayCommand<string>(OpenProjectPartFile, CanOpenFileInSlicer);
    }

    public void OnExpanderItemSelected(ExpanderItemViewModel item)
    {
        ClickedExpanderItem = item;
    }

    private async Task CreateNewProjectTracker()
    {
        ProjectsRootFolder = await folderSelectionService.SelectFolderAsync();
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            return;
        }

        foreach (ExpanderItemViewModel expanderItem in ExpanderItems)
        {
            expanderItem.Dispose();
        }
        ExpanderItems.Clear();

        var projectsTree = fileManagementService.BuildProjectDirectoryTree(ProjectsRootFolder, this);
        projectsTree.IsExpanded = true;
        ExpanderItems.Add(projectsTree);
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

        foreach (var expanderItem in ExpanderItems)
        {
            expanderItem.Dispose();
        }
        ExpanderItems.Clear();

        var loadedProjects = await fileManagementService.LoadProjectsAsync(ProjectsRootFolder, this);
        Debug.WriteLine($"Projects: {loadedProjects.Count}");

        if (loadedProjects.Any())
        {
            foreach (var project in loadedProjects)
                ExpanderItems.Add(project);

            foreach (var item in ExpanderItems)
                item.IsExpanded = true;
        }
        else
        {
            Debug.WriteLine("No existing project data found. Building a new project tree...");
            Debug.WriteLine($"Projects Folder: {ProjectsRootFolder}");
            var projectsTree = fileManagementService.BuildProjectDirectoryTree(ProjectsRootFolder, this);
            projectsTree.IsExpanded = true;
            ExpanderItems.Add(projectsTree);
        }
    }

    private async Task SaveProjects()
    {
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            Debug.WriteLine("No project folder selected.");
            return;
        }

        if (ExpanderItems == null || ExpanderItems.Count == 0)
        {
            Debug.WriteLine("No projects to save.");
            return;
        }

        try
        {
            await fileManagementService.SaveProjectsAsync(ProjectsRootFolder, ExpanderItems);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save projects: {ex.Message}");
        }
    }

    private void OpenProjectPartFile(string partFilePath)
    {
        if (!string.IsNullOrWhiteSpace(partFilePath))
        {
            Debug.WriteLine($"Opening {partFilePath}");
        }
    }

    private bool CanOpenFileInSlicer(string descriptionParameter)
    {
        if (ClickedExpanderItem != null && ClickedExpanderItem.IsProjectFile)
        {
            return true;
        }
        return false;
    }
}