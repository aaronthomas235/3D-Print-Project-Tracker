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

        public Task<PrintModel> GetPrintModelAsync(string filePath)
        {
            if (_cache.TryGetValue(filePath, out var task))
            {
                return task;
            }

            return _cache.GetOrAdd(filePath, _ => Task.Run(() => _modelImportService.ImportModel(filePath)));
        }

        public Task ImportPrintModelAsync(string filePath)
        {
            _cache.GetOrAdd(filePath, _ => Task.Run(() => _modelImportService.ImportModel(filePath)));

            return Task.CompletedTask;
        }

        public void InvalidatePrintModel(string filePath)
        {
            _cache.TryRemove(filePath, out _);
        }
    }
}
