using CommunityToolkit.Mvvm.ComponentModel;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ProjectTreeItemViewModel : ObservableObject
    {
        private readonly ProjectTreeItem _model;
        private readonly IProjectTreeItemViewModelFactory _projectTreeItemViewModelFactory;
        private readonly IPrintModelCacheService _printModelCacheService;
        private readonly IMeshAnalyserService _meshAnalyserService;
        private readonly IPrintTimeEstimationService _printTimeEstimationService;
        private readonly IMaterialUsageEstimationService _materialUsageEstimationService;
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
            private set => SetProperty(ref _printTime, value);
        }

        private string? _materialUsage;
        public string? MaterialUsage
        {
            get => _materialUsage ?? "Estimating Material Usage...";
            set => SetProperty(ref _materialUsage, value);
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

        public ProjectTreeItemViewModel(ProjectTreeItem model, IProjectTreeItemViewModelFactory projectTreeItemViewModelFactory, IPrintModelCacheService printModelCacheService,
            IMeshAnalyserService meshAnalyserService, IPrintTimeEstimationService printTimeEstimationService,
            IMaterialUsageEstimationService materialUsageEstimationService, IPrinterProfileService printerProfileService)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _projectTreeItemViewModelFactory = projectTreeItemViewModelFactory ?? throw new ArgumentNullException(nameof(projectTreeItemViewModelFactory));
            _printModelCacheService = printModelCacheService ?? throw new ArgumentNullException(nameof(printModelCacheService));
            _meshAnalyserService = meshAnalyserService ?? throw new ArgumentNullException(nameof(meshAnalyserService));
            _printTimeEstimationService = printTimeEstimationService ?? throw new ArgumentNullException(nameof(printTimeEstimationService));
            _materialUsageEstimationService = materialUsageEstimationService ?? throw new ArgumentNullException(nameof(materialUsageEstimationService));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));

            Children = new ObservableCollection<ProjectTreeItemViewModel>(_model.Children.Select(childModel => _projectTreeItemViewModelFactory.Create(childModel)));
        }

        public async Task LoadAnalysisAsync(CancellationToken cancellationToken)
        {
            if (!IsFile)
            {
                return; 
            }

            cancellationToken.ThrowIfCancellationRequested();

            var model = await _printModelCacheService.GetPrintModelAsync(Description);
            var profile = ResolvePrinterProfile();

            await Task.WhenAll(
                LoadDimensionsAsync(model, cancellationToken),
                LoadPrintTimeAsync(model, profile, cancellationToken),
                LoadMaterialUsageAsync(model, profile, cancellationToken)
            );
        }

        private async Task LoadDimensionsAsync(PrintModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var dimensions = await _meshAnalyserService.AnalyseMesh(model);
            cancellationToken.ThrowIfCancellationRequested();
            Dimensions = $"{dimensions.Width:F1} x {dimensions.Height:F1} x {dimensions.Depth:F1}";
        }

        private async Task LoadPrintTimeAsync(PrintModel model, PrinterProfile profile, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TimeSpan printTime = await _printTimeEstimationService.EstimatePrintTimeAsync(model, profile);
            cancellationToken.ThrowIfCancellationRequested();
            PrintTime = FormatPrintTimeToString(printTime);
        }

        private async Task LoadMaterialUsageAsync(PrintModel model, PrinterProfile profile, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var materialUsage = "0g";
            MaterialUsage = materialUsage;
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

        private string FormatPrintTimeToString(TimeSpan printTime)
        {
            var totalMinutes = (int)Math.Round(printTime.TotalMinutes);
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
    }
}