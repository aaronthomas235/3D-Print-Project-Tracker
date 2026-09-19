using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.Printing
{
    public interface IMaterialUsageEstimationService
    {
        Task<MaterialEstimate> EstimateAsync(PrintModel model, PrinterProfile profile);
    }
}
