using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factories
{
    public class ProjectTreeItemViewModelFactory : IProjectTreeItemViewModelFactory
    {
        private readonly IMeshAnalyserService _meshAnalyserService;
        private readonly IPrintTimeEstimationService _printTimeEstimationService;
        private readonly IPrinterProfileService _printerProfileService;

        public ProjectTreeItemViewModelFactory(IMeshAnalyserService meshAnalyserService, IPrintTimeEstimationService printTimeEstimationService, IPrinterProfileService printerProfileService)
        {
            _meshAnalyserService = meshAnalyserService ?? throw new ArgumentNullException(nameof(meshAnalyserService));
            _printTimeEstimationService = printTimeEstimationService ?? throw new ArgumentNullException(nameof(printTimeEstimationService));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));
        }

        public ProjectTreeItemViewModel Create(ProjectTreeItem model)
        => new ProjectTreeItemViewModel(model, this, _meshAnalyserService, _printTimeEstimationService, _printerProfileService);
    }
}
