using Core.Models;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPrintTimeEstimationService
    {
        Task<TimeSpan> EstimateAsync(string filePath, PrinterProfile profile);
    }
}
