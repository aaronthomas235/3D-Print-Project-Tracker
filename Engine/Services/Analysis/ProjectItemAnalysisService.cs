using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Interfaces.Analysis;
using ThreeDPrintProjectTracker.Engine.Interfaces.Infrastructure;
using ThreeDPrintProjectTracker.Engine.Interfaces.Models;
using ThreeDPrintProjectTracker.Engine.Interfaces.Printing;
using ThreeDPrintProjectTracker.Engine.Models.Analysis;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Services.Analysis
{
    public class ProjectItemAnalysisService : IPrintItemAnalysisService
    {
        private readonly IPrintModelCacheService _printModelCacheService;
        private readonly IMeshAnalyserService _meshAnalyserService;
        private readonly IPrintTimeEstimationService _printTimeEstimationService;
        private readonly IMaterialUsageEstimationService _materialUsageEstimationService;
        private readonly IPrinterProfileService _printerProfileService;

        public ProjectItemAnalysisService(IPrintModelCacheService printModelCacheService, IMeshAnalyserService meshAnalyserService,
            IPrintTimeEstimationService printTimeEstimationService, IMaterialUsageEstimationService materialUsageEstimationService, IPrinterProfileService printerProfileService)
        {
            _printModelCacheService = printModelCacheService ?? throw new ArgumentNullException(nameof(printModelCacheService));
            _meshAnalyserService = meshAnalyserService ?? throw new ArgumentNullException(nameof(meshAnalyserService));
            _printTimeEstimationService = printTimeEstimationService ?? throw new ArgumentNullException(nameof(printTimeEstimationService));
            _materialUsageEstimationService = materialUsageEstimationService ?? throw new ArgumentNullException(nameof(materialUsageEstimationService));
            _printerProfileService = printerProfileService ?? throw new ArgumentNullException(nameof(printerProfileService));
        }

        public async Task<ProjectItemAnalysisResult> AnalyseAsync(string filePath, Guid printerProfileId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var printModel = await _printModelCacheService.GetPrintModelAsync(filePath);

            token.ThrowIfCancellationRequested();

            var printerProfile = ResolvePrinterProfile(printerProfileId);

            var dimensionsTask = _meshAnalyserService.AnalyseMesh(printModel);
            var printTimeTask = _printTimeEstimationService.EstimatePrintTimeAsync(printModel, printerProfile);
            var materialUsageTask = _materialUsageEstimationService.EstimateAsync(printModel, printerProfile);

            await Task.WhenAll(dimensionsTask, printTimeTask, materialUsageTask);

            token.ThrowIfCancellationRequested();

            return new ProjectItemAnalysisResult
            (
                dimensionsTask.Result,
                printTimeTask.Result,
                materialUsageTask.Result
            );
        }

        private PrinterProfile ResolvePrinterProfile(Guid printerProfileId)
        {
            if (printerProfileId == Guid.Empty)
            {
                return DefaultPrinterProfiles.Default;
            }

            return _printerProfileService.GetPrinterProfileById(printerProfileId)
                   ?? DefaultPrinterProfiles.Default;
        }
    }
}
