using ThreeDPrintProjectTracker.Engine.Models;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IMaterialUsageEstimationService
    {
        Task<MaterialEstimate> EstimateAsync(PrintModel model, PrinterProfile profile);
    }
}
