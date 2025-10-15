using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace _3DPrintProjectTracker;

public partial class MainViewModel : INotifyPropertyChanged
{
    public readonly InterfaceFileManagementService fileManagementService;
    public ObservableCollection<ExpanderItemViewModel> ExpanderItems { get; set; }
    public ICommand OpenProjectsFolderCommand { get; }
    public ICommand SaveCommand { get; }

    private bool _isOption1Checked;
    public bool IsOption1Checked
    {
        get => _isOption1Checked;
        set
        {
            if (_isOption1Checked != value)
            {
                _isOption1Checked = value;
                OnPropertyChanged();
            }
        }
    }

    public MainViewModel(InterfaceFileManagementService interfaceFileManagementService)
    {
        fileManagementService = interfaceFileManagementService;
        ExpanderItems = new ObservableCollection<ExpanderItemViewModel>();
        OpenProjectsFolderCommand = new RelayCommand(OpenProjectsFolder);
        SaveCommand = new RelayCommand(Save);
    }

    [SupportedOSPlatform("windows")]
    void OpenProjectsFolder()
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

        string[] projectDirectories = fileManagementService.GetProjectDirectories(projectsFilePath);

        ExpanderItems.Clear();
        foreach (string projectDirectory in projectDirectories)
        {
            string projectDirectoryName = Path.GetFileName(projectDirectory);

            ExpanderItems.Add(new ExpanderItemViewModel
            {
                Title = projectDirectoryName,
                Description = $"{projectDirectory}",
                IsChecked = false
            });
        }
    }

    private void Save()
    {
        // your save logic
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
