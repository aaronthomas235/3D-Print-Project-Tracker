using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Analysis;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.Analysis
{
    public interface IPrintItemAnalysisService
    {
        Task<ProjectItemAnalysisResult> AnalyseAsync(string filePath, Guid printerProfileId, CancellationToken token);
    }
}
