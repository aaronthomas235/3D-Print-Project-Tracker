using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IFileLauncherService _fileLauncherService;
    private readonly IFolderSelectionService _folderSelectionService;
    private readonly IThemeChangerService _themeChangerService;

    private readonly IProjectTreeCoordinationService _projectTreeCoordinationService;

    public ObservableCollection<ProjectTreeItemViewModel> ProjectTreeItems { get; } = new();

    private ProjectTreeItemViewModel? _clickedProjectTreeItem;
    public ProjectTreeItemViewModel? ClickedProjectTreeItem
    {
        get => _clickedProjectTreeItem;
        set
        {
            if (SetProperty(ref _clickedProjectTreeItem, value))
            {
                _ = HandleClickedProjectTreeItemChangedAsync();
            }
        }
    }

    private string? _projectsRootFolder;
    public string? ProjectsRootFolder
    {
        get => _projectsRootFolder;
        set => SetProperty(ref _projectsRootFolder, value);
    }

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

    private CancellationTokenSource? _selectionCancellationTokenSource;

    public IRelayCommand NewProjectTrackerCommand { get; }
    public IAsyncRelayCommand OpenProjectsFolderAsyncCommand { get; }
    public IAsyncRelayCommand SaveProjectsAsyncCommand { get; }
    public IAsyncRelayCommand OpenSelectedPartCommand { get; }

    public MainViewModel(IFileLauncherService fileLauncherService, IFolderSelectionService folderSelectionService, IThemeChangerService themeChangerService, IProjectTreeCoordinationService projectTreeCoordinationService)
    {
        _fileLauncherService = fileLauncherService ?? throw new ArgumentNullException(nameof(fileLauncherService));
        _folderSelectionService = folderSelectionService ?? throw new ArgumentNullException(nameof(folderSelectionService));
        _themeChangerService = themeChangerService ?? throw new ArgumentNullException(nameof(themeChangerService));
        _projectTreeCoordinationService = projectTreeCoordinationService ?? throw new ArgumentNullException(nameof(projectTreeCoordinationService));

        NewProjectTrackerCommand = new AsyncRelayCommand(CreateNewProjectTracker);
        OpenProjectsFolderAsyncCommand = new AsyncRelayCommand(OpenProjectsFolderAsync);
        SaveProjectsAsyncCommand = new AsyncRelayCommand(SaveProjectsAsync);
        OpenSelectedPartCommand = new AsyncRelayCommand(OpenProjectPartFileAsync);
    }

    private async Task HandleClickedProjectTreeItemChangedAsync()
    {
        try
        {
            await OnClickedProjectTreeItemChanged();
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine($"LoadAnalysisAsync canceled for {ClickedProjectTreeItem?.Title}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load analysis: {ex}");
        }
    }

    private async Task OnClickedProjectTreeItemChanged()
    {
        OpenSelectedPartCommand.NotifyCanExecuteChanged();

        var clickedItem = ClickedProjectTreeItem;

        CancellationToken cancellationToken = BeginNewSelection();

        if (clickedItem is not { IsFile : true })
        {
            clickedItem?.ClearAnalysis();
            return;
        }
        
        await clickedItem.LoadAnalysisAsync(cancellationToken);
    }

    private async Task CreateNewProjectTracker()
    {
        var folder = await _folderSelectionService.SelectFolderAsync();
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        ProjectsRootFolder = folder;

        await LoadProjectsAsync();
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

        await LoadProjectsAsync();
    }

    private async Task SaveProjectsAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder) || ProjectTreeItems.Count == 0)
        {
            return;
        }

        await _projectTreeCoordinationService.SaveProjectsAsync(ProjectsRootFolder, ProjectTreeItems);
    }

    private async Task OpenProjectPartFileAsync()
    {
        if (ClickedProjectTreeItem == null || !ClickedProjectTreeItem.IsFile)
        {
            return;
        }

        await _fileLauncherService.OpenFileAsync(ClickedProjectTreeItem.Description);
    }

    private CancellationToken BeginNewSelection()
    {
        _selectionCancellationTokenSource?.Cancel();
        _selectionCancellationTokenSource = new CancellationTokenSource();
        return _selectionCancellationTokenSource.Token;
    }

    private async Task LoadProjectsAsync()
    {
        ProjectTreeItems.Clear();

        var items = await _projectTreeCoordinationService.LoadProjectsAsync(ProjectsRootFolder!);
        foreach (var item in items)
            ProjectTreeItems.Add(item);
    }
}