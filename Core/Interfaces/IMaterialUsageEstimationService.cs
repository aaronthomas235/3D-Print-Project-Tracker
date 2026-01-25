using Core.Models;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMaterialUsageEstimationService
    {
        Task<MaterialEstimate> EstimateAsync(PrintModel model, PrinterProfile profile);
    }
}
