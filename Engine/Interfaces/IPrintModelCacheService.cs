using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IPrintModelCacheService
    {
        Task<PrintModel> GetPrintModelAsync(string filePath);
        Task<PrintModel> PreloadPrintModelAsync(string filePath);
        void InvalidatePrintModel(string filePath);
    }
}
