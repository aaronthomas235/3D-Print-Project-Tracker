using ThreeDPrintProjectTracker.Engine.Models;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IPrintModelCacheService
    {
        Task<PrintModel> GetPrintModelAsync(string filePath);
        Task<PrintModel> PreloadPrintModelAsync(string filePath);
        void InvalidatePrintModel(string filePath);
    }
}
