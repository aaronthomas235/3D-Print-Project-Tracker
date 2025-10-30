using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Windows.Input;

namespace _3DPrintProjectTracker;

public partial class MainViewModel : INotifyPropertyChanged
{
    public readonly InterfaceFileManagementService fileManagementService;
    public ObservableCollection<ExpanderItemViewModel> ExpanderItems { get; set; }
    public ICommand OpenProjectsFolderCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand OpenSelectedPartCommand { get; }

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

    public MainViewModel(InterfaceFileManagementService interfaceFileManagementService)
    {
        fileManagementService = interfaceFileManagementService;
        ExpanderItems = new ObservableCollection<ExpanderItemViewModel>();
        OpenProjectsFolderCommand = new RelayCommand(OpenProjectsFolder);
        SaveCommand = new RelayCommand(Save);
        OpenSelectedPartCommand = new RelayCommand<string>(OpenProjectPartFile, CanOpenFileInSlicer);
    }

    [SupportedOSPlatform("windows")]
    private void OpenProjectsFolder()
    {
        string projectsFilePath = null;

        using FolderBrowserDialog folderBrowser = new();
        folderBrowser.Description = "Select your projects folder";
        folderBrowser.UseDescriptionForTitle = true;
        DialogResult dialogResult = folderBrowser.ShowDialog();

        if (dialogResult == DialogResult.OK)
        {
            projectsFilePath = folderBrowser.SelectedPath;
        }

        ExpanderItems.Clear();
        ExpanderItemViewModel projectDirectoryTree = BuildProjectDirectoryTree(projectsFilePath);
        projectDirectoryTree.IsExpanded = true;
        ExpanderItems.Add(projectDirectoryTree);
    }

    private ExpanderItemViewModel BuildProjectDirectoryTree(string ProjectFilePath)
    {
        string projectName = Path.GetFileName(ProjectFilePath);
        string[] subDirectories = fileManagementService.GetProjectDirectories(ProjectFilePath);
        string[] directoryFiles = fileManagementService.GetProjectFiles(ProjectFilePath);


        ExpanderItemViewModel projectDirectoryTree = new ExpanderItemViewModel(this)
        {
            Title = projectName,
            Description = ProjectFilePath,
            IsChecked = false,
            IsProjectFile = false,
        };

        foreach (string subDirectory in subDirectories)
        {
            projectDirectoryTree.Children.Add(BuildProjectDirectoryTree(subDirectory));
        }

        foreach (string directoryFile in directoryFiles)
        {
            string directoryFileName = Path.GetFileName(directoryFile);
            projectDirectoryTree.Children.Add(new ExpanderItemViewModel(this)
            {
                Title = directoryFileName,
                Description = directoryFile,
                IsChecked = false,
                IsProjectFile = true,
            });
        }

        return projectDirectoryTree;
    }

    private void Save()
    {
        // save logic
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
