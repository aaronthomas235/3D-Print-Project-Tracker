using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ProjectTreeItemViewModel : ObservableObject, IDisposable
    {
        private IProjectTreeItemHost? _projectTreeItemHost;
        private IPrinterProfileService _printerProfileService;
        private IMeshAnalyserService _meshAnalyserService;
        private IPrintTimeEstimationService _printTimeEstimationService;

        private bool _isUpdatingChildren;
        private bool _isDisposed;

        private bool _areDimensionsLoaded = false;
        private bool _hasPrintTimeBeenEstimated = false;

        private string _title = String.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description = String.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private Guid _assignedPrinterProfileId = Guid.Empty;

        public Guid AssignedPrinterProfileId
        {
            get => _assignedPrinterProfileId;
            set
    {
                if (SetProperty(ref _assignedPrinterProfileId, value))
                {
                    _hasPrintTimeBeenEstimated = false;
                    PrintTime = String.Empty;
                }
            }
        }

        private string _partName = String.Empty;
        public string PartName
        {
            get => _partName;
            set => SetProperty(ref _partName, value);
        }

        private string _dimensions = String.Empty;
        public string Dimensions
        {
            get => _dimensions;
            set => SetProperty(ref _dimensions, value);
        }

        private string _printTime = String.Empty;
        public string PrintTime
        {
            get => _printTime;
            set => SetProperty(ref _printTime, value);
        }

        private string _materialUsage = String.Empty;
        public string MaterialUsage
        {
            get => _materialUsage;
            set => SetProperty(ref _materialUsage, value);
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isProjectFile;
        public bool IsProjectFile
        {
            get => _isProjectFile;
            set
            {
                if (SetProperty(ref _isProjectFile, value))
                {
                    ShowPartDetailsCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool? _isChecked;
        public bool? IsChecked
        {
            get => _isChecked;
            set
            {
                if (SetProperty(ref _isChecked, value))
                {
                    OnIsCheckedChanged();
                }
            }
        }

        [JsonInclude]
        public ObservableCollection<ProjectTreeItemViewModel> Children { get; private set; } = new();

        [JsonIgnore]
        public IRelayCommand ShowPartDetailsCommand { get; private set; }


        public ProjectTreeItemViewModel(IProjectTreeItemHost projectTreeItemHost, IPrinterProfileService printerProfileService, IMeshAnalyserService meshAnalyserService, IPrintTimeEstimationService printTimeEstimationService)
        {
            _projectTreeItemHost = projectTreeItemHost ?? throw new ArgumentNullException(nameof(projectTreeItemHost));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));
            _meshAnalyserService = meshAnalyserService ?? throw new ArgumentNullException(nameof(meshAnalyserService));
            _printTimeEstimationService = printTimeEstimationService ?? throw new ArgumentNullException(nameof(printTimeEstimationService));

            ShowPartDetailsCommand = new RelayCommand(ShowPartDetails, () => IsProjectFile);
            InitialiseCommonResources(projectTreeItemHost);
        }

        private void InitialiseCommonResources(IProjectTreeItemHost? projectTreeItemHost)
        {
            _projectTreeItemHost = projectTreeItemHost;

            Children.CollectionChanged += OnChildrenCollectionChanged;
            foreach(var child in Children)
            {
                child.InitialiseCommonResources(projectTreeItemHost);
            }
        }

        public void InitialiseRuntimeResources(IProjectTreeItemHost projectTreeItemHost)
        {
            InitialiseCommonResources(projectTreeItemHost);
        }

        private void OnChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems != null)
            {
                foreach (ProjectTreeItemViewModel child in eventArgs.NewItems)
                {
                    child.PropertyChanged += OnChildPropertyChanged;
                }
            }

            if (eventArgs.OldItems != null)
            {
                foreach (ProjectTreeItemViewModel child in eventArgs.OldItems)
                {
                    child.PropertyChanged -= OnChildPropertyChanged;
                }
            }
        }

        private void OnChildPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
        {
            if (_isUpdatingChildren || eventArgs.PropertyName != nameof(IsChecked))
            {
                return;
            }

            UpdateCheckStateFromChildren();
        }

        private void UpdateCheckStateFromChildren()
        {
            if (IsProjectFile || Children.Count == 0)
            {
                return;
            }

            bool? newCheckState = CalculateCheckStateFromChildren();

            if (_isChecked != newCheckState)
            {
                SetProperty(ref _isChecked, newCheckState, nameof(IsChecked));
            }
        }

        private bool? CalculateCheckStateFromChildren()
        {
            if (Children.All(c => c.IsChecked == true))
            {
                return true;
            }
            if (Children.All(c => c.IsChecked == false))
            {
                return false;
            }
            return null;
        }

        private void OnIsCheckedChanged()
        {
            if (IsProjectFile)
            {
                if (IsChecked == null)
                {
                    IsChecked = false;
                }
                return;
            }

            if (IsChecked.HasValue)
            {
                _isUpdatingChildren = true;
                try
                {
                    foreach(var child in Children)
                    {
                        child.IsChecked = IsChecked;
                    }
                }
                finally
                {
                    _isUpdatingChildren = false;
                }
            }
        }

        private PrinterProfile ResolvePrinterProfile()
        {
            if (AssignedPrinterProfileId != Guid.Empty)
            {
                var profile = _printerProfileService.GetPrinterProfileById(AssignedPrinterProfileId);
                if (profile != null)
                {
                    return profile;
                }
            }

            return ReferencePrinterProfile.Default;
        }

        public async Task LoadDimensionsAsync()
        {
            if (!IsProjectFile || _areDimensionsLoaded)
            {
                return;
            }

            try
            {
                var calculatedDimensions = await _meshAnalyserService.AnalyseAsync(Description);
                Dimensions = $"{calculatedDimensions.Width:F1} x {calculatedDimensions.Height:F1} x {calculatedDimensions.Depth:F1}";
            }
            catch
            {
                Dimensions = "0 x 0 x 0";
            }
        }

        public async Task LoadPrintTimeAsync()
        {
            if(!IsProjectFile || _hasPrintTimeBeenEstimated)
            {
                return;
            }

            try
            {
                var selectedProfile = ResolvePrinterProfile();
                Debug.WriteLine($"Printer Name: {selectedProfile.Name}");
                var timeSpan = await _printTimeEstimationService.EstimateAsync(Description, selectedProfile);

                PrintTime = FormatTimeSpan(timeSpan);
                _hasPrintTimeBeenEstimated = true;
            }
            catch
            {
                PrintTime = "0 Days 0 Hours 0 Minutes";
            }
        }

        private static string FormatTimeSpan(TimeSpan calculatedTimeSpan)
        {
            var totalMinutes = (int)Math.Round(calculatedTimeSpan.TotalMinutes);
            if (totalMinutes < 0)
            {
                totalMinutes = 0;
            }

            var days = totalMinutes / (24 * 60);
            var hours = (totalMinutes % (24 * 60)) / 60;
            var minutes = totalMinutes % 60;

            if (days > 0)
            {
                return $"{days} Days {hours} Hours {minutes} Minutes";
            }
            return $"{hours} Hours {minutes} Minutes";
        }

        private void ShowPartDetails()
        {
            _projectTreeItemHost?.OnProjectTreeItemSelected(this);
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            foreach (var child in Children)
                child.PropertyChanged -= OnChildPropertyChanged;

            Children.CollectionChanged -= OnChildrenCollectionChanged;
        }
    }
}