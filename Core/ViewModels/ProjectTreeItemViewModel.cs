using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using Core.Models;
using Core.Services;
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
    public class ProjectTreeItemViewModel : ObservableObject
    {
        private readonly ProjectTreeItem _model;
        private readonly IProjectTreeItemViewModelFactory _projectTreeItemViewModelFactory;
        private readonly IMeshAnalyserService _meshAnalyserService;
        private readonly IPrintTimeEstimationService _printTimeEstimationService;
        private readonly IPrinterProfileService _printerProfileService;

        public ObservableCollection<ProjectTreeItemViewModel> Children { get; }

        public string Title
        {
            get => _model.Title;
            set => SetProperty(_model.Title, value, v => _model.Title = v);
        }

        public string Description
        {
            get => _model.Description;
            set => SetProperty(_model.Description, value, v => _model.Description = v);
        }

        private string? _dimensions;
        public string? Dimensions
        {
            get => _dimensions ?? "Measuring Dimensions...";
            private set => SetProperty(ref _dimensions, value);
        }

        private string? _printTime;
        public string? PrintTime
        {
            get => _printTime ?? "Estimating Print Time...";
            private set => SetProperty(ref _dimensions, value);
        }

        public bool IsFile => _model.IsFile;

        public Guid AssignedPrinterProfileId
        {
            get => _model.AssignedPrinterProfileId;
            set => SetProperty(_model.AssignedPrinterProfileId, value, v => _model.AssignedPrinterProfileId = v);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (SetProperty(ref _isCompleted, value))
                {
                    foreach (var child in Children)
                    {
                        child.IsCompleted = value;
                    }
                }
            }
        }

        public ProjectTreeItemViewModel(ProjectTreeItem model, IProjectTreeItemViewModelFactory projectTreeItemViewModelFactory, IMeshAnalyserService meshAnalyserService, IPrintTimeEstimationService printTimeEstimationService, IPrinterProfileService printerProfileService)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _projectTreeItemViewModelFactory = projectTreeItemViewModelFactory ?? throw new ArgumentNullException(nameof(projectTreeItemViewModelFactory));
            _meshAnalyserService = meshAnalyserService ?? throw new ArgumentNullException(nameof(meshAnalyserService));
            _printTimeEstimationService = printTimeEstimationService ?? throw new ArgumentNullException(nameof(printTimeEstimationService));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));

            Children = new ObservableCollection<ProjectTreeItemViewModel>(_model.Children.Select(childModel => _projectTreeItemViewModelFactory.Create(childModel)));
        }

        public async Task LoadDimensionsAsync()
        {
            if (!IsFile)
            {
                return;
            }

            var dims = await _meshAnalyserService.AnalyseAsync(Description);
            Dimensions = $"{dims.Width:F1} x {dims.Height:F1} x {dims.Depth:F1}";
        }

        public async Task LoadPrintTimeAsync()
        {
            if (!IsFile)
            {
                return;
            }

            var profile = ResolvePrinterProfile();
            var time = await _printTimeEstimationService.EstimateAsync(Description, profile);
            PrintTime = time.ToString();
        }

        public ProjectTreeItem ToModel()
        {
            return new ProjectTreeItem
            {
                Title = Title,
                Description = Description,
                IsFile = IsFile,
                AssignedPrinterProfileId = AssignedPrinterProfileId,
                Children = Children.Select(c => c.ToModel()).ToList()
            };
        }

        private PrinterProfile ResolvePrinterProfile()
        {
            if (AssignedPrinterProfileId == Guid.Empty)
            {
                return ReferencePrinterProfile.Default;
            }

            return _printerProfileService.GetPrinterProfileById(AssignedPrinterProfileId) ?? ReferencePrinterProfile.Default;
        }
    }
}