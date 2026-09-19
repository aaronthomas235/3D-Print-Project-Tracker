using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Geometry;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Models.Analysis
{
    public record ProjectItemAnalysisResult(
        MeshDimensions? Dimensions,
        TimeSpan? PrintTime,
        MaterialEstimate? MaterialUsage
    );
}
