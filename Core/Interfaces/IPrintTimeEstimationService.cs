using Core.Models;
using System;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPrintTimeEstimationService
    {
        Task<TimeSpan> EstimatePrintTimeAsync(PrintModel model, PrinterProfile profile);
    }
}
