using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Interfaces.Analysis;
using ThreeDPrintProjectTracker.Engine.Models.Geometry;
using ThreeDPrintProjectTracker.Engine.Models.Printing;
using ThreeDPrintProjectTracker.Engine.Models.Projects;

namespace ThreeDPrintProjectTracker.Avalonia.ViewModels
{
    public partial class ProjectTreeItemViewModel : ObservableObject
    {
        private readonly ProjectTreeItemViewModel? _parent;
        private readonly ProjectTreeItem _model;
        private readonly IPrintItemAnalysisService _analysisService;

        public ObservableCollection<ProjectTreeItemViewModel> Children { get; }

        public string Title => _model.Title;
        public string FilePath => _model.FilePath;

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private bool isCompleted;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DimensionsDisplay))]
        private MeshDimensions? dimensions;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PrintTimeDisplay))]
        private TimeSpan? printTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MaterialUsageDisplay))]
        private MaterialEstimate? materialUsage;


        public string? DimensionsDisplay => Dimensions is null
            ? (IsAnalysable ? "Measuring Dimensions..." : null)
            : $"{Dimensions.Width:F1} x {Dimensions.Height:F1} x {Dimensions.Depth:F1} mm";

        public string? PrintTimeDisplay => PrintTime is null
        ? (IsAnalysable ? "Estimating Print Time..." : null)
        : FormatPrintTime(PrintTime.Value);

        public string? MaterialUsageDisplay => MaterialUsage is null
        ? (IsAnalysable ? "Estimating Material Usage..." : null)
        : $"{MaterialUsage.WeightGrams:F0} g";

        public bool IsFile => _model.IsFile;
        private bool IsAnalysable => IsFile;

        public Guid AssignedPrinterProfileId
        {
            get => _model.AssignedPrinterProfileId;
            set => SetProperty(_model.AssignedPrinterProfileId, value, v => _model.AssignedPrinterProfileId = v);
        }

        

        private int _analysisVersion;

        public ProjectTreeItemViewModel(ProjectTreeItem model, IPrintItemAnalysisService analysisService, ProjectTreeItemViewModel? parent)
        {
            _parent = parent;
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));

            Children = new ObservableCollection<ProjectTreeItemViewModel>();
        }

        public void ClearAnalysis()
        {
            Dimensions = null;
            PrintTime = null;
            MaterialUsage = null;
        }

        public async Task LoadAnalysisAsync(CancellationToken cancellationToken)
        {
            if (!IsAnalysable)
            {
                return;
            }

            var version = ++_analysisVersion;

            ClearAnalysis();

            var result = await _analysisService.AnalyseAsync(FilePath, AssignedPrinterProfileId, cancellationToken);

            if (version != _analysisVersion)
            {
                return;
            }

            Dimensions = result.Dimensions;
            PrintTime = result.PrintTime;
            MaterialUsage = result.MaterialUsage;
        }

        public void CollapseAll()
        {
            IsExpanded = false;
            foreach (var child in Children)
            {
                child.CollapseAll();
            }
        }

        public ProjectTreeItem ToModelRecursive()
        {
            return new ProjectTreeItem
            {
                Title = Title,
                FilePath = FilePath,
                IsFile = IsFile,
                AssignedPrinterProfileId = AssignedPrinterProfileId,
                Children = Children.Select(c => c.ToModelRecursive()).ToList()
            };
        }

        private static string FormatPrintTime(TimeSpan time)
        {
            var totalMinutes = (int)Math.Round(time.TotalMinutes);

            var days = totalMinutes / (24 * 60);
            var hours = (totalMinutes % (24 * 60)) / 60;
            var minutes = totalMinutes % 60;

            var parts = new List<string>();

            if (days > 0)
            {
                parts.Add($"{days} d");
            }

            if (hours > 0)
            {
                parts.Add($"{hours} h");
            }

            if (minutes > 0 || parts.Count == 0)
            {
                parts.Add($"{minutes} min");
            }

            return string.Join(" ", parts);
        }

        partial void OnIsCompletedChanged(bool value)
        {
            foreach (var child in Children)
            {
                if (child.IsCompleted != value)
                {
                    child.IsCompleted = value;
                }
            }

            _parent?.UpdateCompletionFromChildren();
        }

        private void UpdateCompletionFromChildren()
        {
            if (Children.Count == 0)
            {
                return;
            }

            var allCompleted = Children.All(c => c.IsCompleted);

            if (IsCompleted != allCompleted)
            {
                IsCompleted = allCompleted;
            }
        }
    }
}