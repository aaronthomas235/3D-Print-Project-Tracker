using Core.Models;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPrintModelCacheService
    {
        Task<PrintModel> GetPrintModelAsync(string filePath);
        Task ImportPrintModelAsync(string filePath);
        void InvalidatePrintModel(string filePath);
    }
}
