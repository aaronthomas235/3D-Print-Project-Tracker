using _3DPrintProjectTracker.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Windows.Input;

namespace _3DPrintProjectTracker.ViewModels;

public partial class MainViewModel : INotifyPropertyChanged
{
    public readonly IFileManagementService fileManagementService;
    public readonly IFolderSelectionService folderSelectionService;
    public ObservableCollection<ExpanderItemViewModel> ExpanderItems { get; set; }
    public ICommand OpenProjectsFolderCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand OpenSelectedPartCommand { get; }

    private string _projectsRootFolder;
    public string ProjectsRootFolder
    {
        get => _projectsRootFolder;
        set
        {
            if (_projectsRootFolder != value)
            {
                _projectsRootFolder = value;
                OnPropertyChanged();
            }
        }
    }

    private ExpanderItemViewModel _clickedExpanderItem;
    public ExpanderItemViewModel ClickedExpanderItem
    {
        get => _clickedExpanderItem;
        set
        {
            if (_clickedExpanderItem != value)
            {
                _clickedExpanderItem = value;
                OnPropertyChanged();
                (OpenSelectedPartCommand as RelayCommand<string>)?.NotifyCanExecuteChanged();
            }
        }
    }

    public MainViewModel(IFileManagementService fileManagementService, IFolderSelectionService folderSelectionService)
    {
        this.fileManagementService = fileManagementService;
        this.folderSelectionService = folderSelectionService;

        ExpanderItems = new ObservableCollection<ExpanderItemViewModel>();

        OpenProjectsFolderCommand = new RelayCommand(OpenProjectsFolder);
        SaveCommand = new RelayCommand(SaveProjects);
        OpenSelectedPartCommand = new RelayCommand<string>(OpenProjectPartFile, CanOpenFileInSlicer);
    }

    private void OpenProjectsFolder()
    {
        string projectsRootFolderPath = folderSelectionService.SelectFolder("Select your projects folder");
        if (string.IsNullOrWhiteSpace(projectsRootFolderPath))
        {
            return;
        }

        ProjectsRootFolder = projectsRootFolderPath;

        ExpanderItems.Clear();

        var projectsTree = fileManagementService.BuildProjectDirectoryTree(projectsRootFolderPath, this);
        projectsTree.IsExpanded = true;
        ExpanderItems.Add(projectsTree);
    }

    private void SaveProjects()
    {
        if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
        {
            ProjectsRootFolder = folderSelectionService.SelectFolder("Select folder to save projects");
            if (string.IsNullOrWhiteSpace(ProjectsRootFolder))
                return;
        }

        if (ExpanderItems.Count == 0)
        {
            return;
        }
        fileManagementService.SaveProjectsAsync(ProjectsRootFolder, ExpanderItems);
    }

    private void OpenProjectPartFile(object descriptionParameter)
    {
        string partFilePath = descriptionParameter as string;
        System.Windows.MessageBox.Show($"Opening {partFilePath}");
    }

    private bool CanOpenFileInSlicer(string descriptionParameter)
    {
        if (ClickedExpanderItem != null && ClickedExpanderItem.IsProjectFile)
        {
            return true;
        }
        return false;
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
