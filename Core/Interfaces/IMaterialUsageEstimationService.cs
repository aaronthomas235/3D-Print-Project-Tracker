using Core.Models;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMaterialUsageEstimationService
    {
        Task<string> EstimateWeightAsync(string filePath, PrinterProfile profile);
    }
}
