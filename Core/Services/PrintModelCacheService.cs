using Core.Interfaces;
using Core.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Core.Services
{
    public class PrintModelCacheService : IPrintModelCacheService
    {
        private readonly IPrintModelImportService _modelImportService;
        private readonly ConcurrentDictionary<string, Task<PrintModel>> _cache = new();

        public PrintModelCacheService(IPrintModelImportService modelImportService)
        {
            _modelImportService = modelImportService;
        }

        public async Task<PrintModel> GetPrintModelAsync(string filePath)
        {
            try
            {
                return await _cache.GetOrAdd(
                    filePath,
                    _ => Task.Run(() => _modelImportService.ImportModel(filePath))
                );
            }
            catch
            {
                _cache.TryRemove(filePath, out _);
                throw;
            }
        }

        public Task<PrintModel> PreloadPrintModelAsync(string filePath)
        {
            return GetPrintModelAsync(filePath);
        }

        public void InvalidatePrintModel(string filePath)
        {
            _cache.TryRemove(filePath, out _);
        }
    }
}
