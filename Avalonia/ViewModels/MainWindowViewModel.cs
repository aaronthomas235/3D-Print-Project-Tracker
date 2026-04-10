using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Avalonia.Interfaces;
using ThreeDPrintProjectTracker.Engine.Interfaces;
using ThreeDPrintProjectTracker.Engine.Interfaces.Printing;
using ThreeDPrintProjectTracker.Engine.Interfaces.UI;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IFileLauncherService _fileLauncherService;
        private readonly IFolderSelectionService _folderSelectionService;
        private readonly IThemeChangerService _themeChangerService;
        private readonly IWindowCreationService _windowCreationService;
        private readonly IProjectTreeItemViewModelFactory _projectTreeItemViewModelFactory;
        private readonly IProjectTreeCoordinationService _projectTreeCoordinationService;
        private readonly IPrinterProfileService _printerProfileService;

        public ObservableCollection<ProjectTreeItemViewModel> ProjectTreeItems { get; } = new();
        public ObservableCollection<PrinterProfile> PrinterProfiles { get; } = new();

        [ObservableProperty]
        private PrinterProfile? selectedPrinterProfile;

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

        public IRelayCommand<Window> NewProjectTrackerCommand { get; }
        public IAsyncRelayCommand<Window> OpenProjectsFolderAsyncCommand { get; }
        public IAsyncRelayCommand SaveProjectsAsyncCommand { get; }
        public IAsyncRelayCommand ManageFilamentsAsyncCommand { get; }
        public IAsyncRelayCommand ManagePrintersAsyncCommand { get; }
        public IAsyncRelayCommand OpenSelectedPartCommand { get; }

        public MainWindowViewModel(IFileLauncherService fileLauncherService, IFolderSelectionService folderSelectionService,
        IThemeChangerService themeChangerService, IWindowCreationService windowCreationService, IProjectTreeItemViewModelFactory projectTreeItemViewModelFactory,
        IProjectTreeCoordinationService projectTreeCoordinationService, IPrinterProfileService printerProfileService)
        {
            _fileLauncherService = fileLauncherService ?? throw new ArgumentNullException(nameof(fileLauncherService));
            _folderSelectionService = folderSelectionService ?? throw new ArgumentNullException(nameof(folderSelectionService));
            _themeChangerService = themeChangerService ?? throw new ArgumentNullException(nameof(themeChangerService));
            _windowCreationService = windowCreationService ?? throw new ArgumentNullException(nameof(windowCreationService));
            _projectTreeItemViewModelFactory = projectTreeItemViewModelFactory ?? throw new ArgumentNullException(nameof(projectTreeItemViewModelFactory));
            _projectTreeCoordinationService = projectTreeCoordinationService ?? throw new ArgumentNullException(nameof(projectTreeCoordinationService));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));

            NewProjectTrackerCommand = new AsyncRelayCommand<Window>(CreateNewProjectTracker);
            OpenProjectsFolderAsyncCommand = new AsyncRelayCommand<Window>(OpenProjectsFolderAsync);
            SaveProjectsAsyncCommand = new AsyncRelayCommand(SaveProjectsAsync);
            ManageFilamentsAsyncCommand = new AsyncRelayCommand(ShowManageFilamentsAsync);
            ManagePrintersAsyncCommand = new AsyncRelayCommand(ShowManagePrintersAsync);
            OpenSelectedPartCommand = new AsyncRelayCommand(OpenProjectPartFileAsync);

            ReloadProfiles();
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

            if (clickedItem is not { IsFile: true })
            {
                clickedItem?.ClearAnalysis();
                return;
            }

            await clickedItem.LoadAnalysisAsync(cancellationToken);
        }

        private async Task CreateNewProjectTracker(Window? window)
        {
            if (window is null)
            {
                return;
            }

            var folder = await _folderSelectionService.SelectFolderAsync(window);

            if (string.IsNullOrWhiteSpace(folder))
            {
                return;
            }

            ProjectsRootFolder = folder;

            await LoadProjectsAsync();
        }

        private async Task OpenProjectsFolderAsync(Window? window)
        {
            if (window is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
            {
                ProjectsRootFolder = await _folderSelectionService.SelectFolderAsync(window);

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

            var models = ProjectTreeItems
                .Select(vm => vm.ToModelRecursive())
                .ToList();

            await _projectTreeCoordinationService.SaveProjectsAsync(ProjectsRootFolder, models);
        }

        private async Task ShowManageFilamentsAsync()
        {
            await _windowCreationService.ShowManageFilamentsAsync();
        }

        private async Task ShowManagePrintersAsync()
        {
            await _windowCreationService.ShowManagePrintersAsync();

            ReloadProfiles();
        }

        private async Task OpenProjectPartFileAsync()
        {
            if (ClickedProjectTreeItem == null || !ClickedProjectTreeItem.IsFile)
            {
                return;
            }

            await _fileLauncherService.OpenFileAsync(ClickedProjectTreeItem.FilePath);
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

            var models = await _projectTreeCoordinationService.LoadProjectsAsync(ProjectsRootFolder!);

            foreach (var model in models)
            {
                var vm = _projectTreeItemViewModelFactory.Create(model);
                vm.CollapseAll();
                ProjectTreeItems.Add(vm);
            }
        }

        private void ReloadProfiles()
        {
            var currentProfileId = SelectedPrinterProfile?.Id;

            PrinterProfiles.Clear();

            foreach (var profile in _printerProfileService.GetAllPrinterProfiles())
            {
                PrinterProfiles.Add(profile);
            }

            SelectedPrinterProfile = PrinterProfiles.FirstOrDefault(p => p.Id == currentProfileId) ?? PrinterProfiles.FirstOrDefault();

        }
    }
}
