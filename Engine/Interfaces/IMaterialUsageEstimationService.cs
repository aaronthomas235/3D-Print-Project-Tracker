using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IMaterialUsageEstimationService
    {
        Task<MaterialEstimate> EstimateAsync(PrintModel model, PrinterProfile profile);
    }
}
