using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.ViewModels;

public partial class MainViewModel : ObservableObject, IProjectTreeItemHost
{
    private readonly IFileLauncherService _fileLauncherService;
    private readonly IFileManagementService _fileManagementService;
    private readonly IFolderSelectionService _folderSelectionService;
    private readonly IMeshAnalyserService _meshAnalyserService;
    private readonly IPrinterProfileService _printerProfileService;
    private readonly IPrintTimeEstimationService _printTimeEstimationService;
    private readonly ISupportedFileFormatsService _supportedFileFormatsService;
    private readonly IThemeChangerService _themeChangerService;

    public ObservableCollection<ProjectTreeItemViewModel> ProjectTreeItems { get; } = new();

    public IRelayCommand NewProjectTrackerCommand { get; }
    public IAsyncRelayCommand OpenProjectsFolderAsyncCommand { get; }
    public IAsyncRelayCommand SaveProjectsAsyncCommand { get; }
    public IAsyncRelayCommand OpenSelectedPartCommand { get; }

    private bool _isUsingDarkTheme = true;
    public bool IsUsingDarkTheme
    {
        get => _isUsingDarkTheme;
        set
        {
            if (SetProperty(ref _isUsingDarkTheme, value))
            {
                _themeChangerService.SetTheme(value);
            }
        }
    }

    private string? _projectsRootFolder;
    public string? ProjectsRootFolder
    {
        get => _projectsRootFolder;
        set => SetProperty(ref _projectsRootFolder, value);
    }

    private ProjectTreeItemViewModel? _clickedProjectTreeItem;
    public ProjectTreeItemViewModel? ClickedProjectTreeItem
    {
        get => _clickedProjectTreeItem;
        set
        {
            if (SetProperty(ref _clickedProjectTreeItem, value))
            {
                OnClickedProjectTreeItemChanged(value);
            }
        }
    }

    public MainViewModel(IFileLauncherService fileLauncherService, IFileManagementService fileManagementService, IFolderSelectionService folderSelectionService,
        IMeshAnalyserService meshAnalyserService, IPrinterProfileService printerProfileService, IPrintTimeEstimationService printTimeEstimationService,
        ISupportedFileFormatsService supportedFileFormatsService, IThemeChangerService themeChangerService)
    {
        _fileLauncherService = fileLauncherService;
        _fileManagementService = fileManagementService;
        _folderSelectionService = folderSelectionService;
        _meshAnalyserService = meshAnalyserService;
        _printerProfileService = printerProfileService;
        _printTimeEstimationService = printTimeEstimationService;
        _supportedFileFormatsService = supportedFileFormatsService;
        _themeChangerService = themeChangerService;

        NewProjectTrackerCommand = new AsyncRelayCommand(CreateNewProjectTracker);
        OpenProjectsFolderAsyncCommand = new AsyncRelayCommand(OpenProjectsFolderAsync);
        SaveProjectsAsyncCommand = new AsyncRelayCommand(SaveProjectsAsync);
        OpenSelectedPartCommand = new AsyncRelayCommand(OpenProjectPartFileAsync);
    }

    private async void OnClickedProjectTreeItemChanged(ProjectTreeItemViewModel? item)
    {
        if (item == null)
            return;

        _ = item.LoadDimensionsAsync();
        _ = item.LoadPrintTimeAsync();

        OpenSelectedPartCommand.NotifyCanExecuteChanged();
    }

    public void OnProjectTreeItemSelected(ProjectTreeItemViewModel item)
    {
        if (item == null)
        {
            return;
        }

        _ = item.LoadDimensionsAsync();
        _ = item.LoadPrintTimeAsync();
        ClickedProjectTreeItem = item;
        OpenSelectedPartCommand.NotifyCanExecuteChanged();
    }

    private async Task CreateNewProjectTracker()
    {
        ProjectsRootFolder = await _folderSelectionService.SelectFolderAsync();
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            return;
        }

        foreach (ProjectTreeItemViewModel projectTreeItem in ProjectTreeItems)
        {
            projectTreeItem.Dispose();
        }
        ProjectTreeItems.Clear();

        var projectsTreeItems = _fileManagementService.BuildProjectDirectoryTree(ProjectsRootFolder, this, _meshAnalyserService, _printerProfileService, _printTimeEstimationService);

        foreach(var projectTreeItem in projectsTreeItems)
        {
            CollapseChildren(projectTreeItem);
            projectTreeItem.IsExpanded = false;
            ProjectTreeItems.Add(projectTreeItem);
        }
    }

    private async Task OpenProjectsFolderAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            ProjectsRootFolder = await _folderSelectionService.SelectFolderAsync();
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

        var loadedProjects = await _fileManagementService.LoadProjectsAsync(ProjectsRootFolder, this);
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
            var projectsTreeItems = _fileManagementService.BuildProjectDirectoryTree(ProjectsRootFolder, this, _meshAnalyserService, _printerProfileService, _printTimeEstimationService);

            foreach (var projectTreeItem in projectsTreeItems)
            {
                CollapseChildren(projectTreeItem);
                projectTreeItem.IsExpanded = false;
                ProjectTreeItems.Add(projectTreeItem);
            }
        }
    }

    private async Task SaveProjectsAsync()
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
            await _fileManagementService.SaveProjectsAsync(ProjectsRootFolder, ProjectTreeItems);
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

    private async Task OpenProjectPartFileAsync()
    {
        if (ClickedProjectTreeItem == null || !ClickedProjectTreeItem.IsProjectFile)
        {
            return;
        }

        try
        {
            await _fileLauncherService.OpenFileAsync(ClickedProjectTreeItem.Description);
        }
        catch (Exception ex) {
            Debug.WriteLine($"Failed to open file: {ex.Message}");
        }
    }
}