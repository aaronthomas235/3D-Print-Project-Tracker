using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using System;

namespace Core.Factories
{
    public class ProjectTreeItemViewModelFactory : IProjectTreeItemViewModelFactory
    {
        private readonly IMeshAnalyserService _meshAnalyserService;
        private readonly IPrintTimeEstimationService _printTimeEstimationService;
        private readonly IMaterialUsageEstimationService _materialUsageEstimationService;
        private readonly IPrinterProfileService _printerProfileService;

        public ProjectTreeItemViewModelFactory(IMeshAnalyserService meshAnalyserService, IPrintTimeEstimationService printTimeEstimationService, IMaterialUsageEstimationService materialUsageEstimationService, IPrinterProfileService printerProfileService)
        {
            _meshAnalyserService = meshAnalyserService ?? throw new ArgumentNullException(nameof(meshAnalyserService));
            _printTimeEstimationService = printTimeEstimationService ?? throw new ArgumentNullException(nameof(printTimeEstimationService));
            _materialUsageEstimationService = materialUsageEstimationService ?? throw new ArgumentNullException(nameof(materialUsageEstimationService));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));
        }

        public ProjectTreeItemViewModel Create(ProjectTreeItem model)
        => new ProjectTreeItemViewModel(model, this, _meshAnalyserService, _printTimeEstimationService, _materialUsageEstimationService, _printerProfileService);
    }
}
