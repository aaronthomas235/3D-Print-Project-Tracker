using ThreeDPrintProjectTracker.Engine.Interfaces;
using System;
using ThreeDPrintProjectTracker.Engine.Models.Projects;
using ThreeDPrintProjectTracker.Engine.Interfaces.Printing;
using ThreeDPrintProjectTracker.Engine.Interfaces.Models;
using ThreeDPrintProjectTracker.Engine.Interfaces.Infrastructure;
using ThreeDPrintProjectTracker.Avalonia.Interfaces;
using ThreeDPrintProjectTracker.Avalonia.ViewModels;

namespace ThreeDPrintProjectTracker.Avalonia.Factories
{
    public class ProjectTreeItemViewModelFactory : IProjectTreeItemViewModelFactory
    {
        private readonly IPrintModelCacheService _printModelCacheService;
        private readonly IMeshAnalyserService _meshAnalyserService;
        private readonly IPrintTimeEstimationService _printTimeEstimationService;
        private readonly IMaterialUsageEstimationService _materialUsageEstimationService;
        private readonly IPrinterProfileService _printerProfileService;

        public ProjectTreeItemViewModelFactory(
            IPrintModelCacheService printModelCacheService,
            IMeshAnalyserService meshAnalyserService,
            IPrintTimeEstimationService printTimeEstimationService,
            IMaterialUsageEstimationService materialUsageEstimationService,
            IPrinterProfileService printerProfileService)
        {
            _printModelCacheService = printModelCacheService;
            _meshAnalyserService = meshAnalyserService;
            _printTimeEstimationService = printTimeEstimationService;
            _materialUsageEstimationService = materialUsageEstimationService;
            _printerProfileService = printerProfileService;
        }

        public ProjectTreeItemViewModel Create(ProjectTreeItem model)
        {
            return new ProjectTreeItemViewModel(
                model,
                _printModelCacheService,
                _meshAnalyserService,
                _printTimeEstimationService,
                _materialUsageEstimationService,
                _printerProfileService
            );
        }
    }
}
