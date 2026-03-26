using ThreeDPrintProjectTracker.Engine.Models;
using System;
using System.Threading.Tasks;

namespace ThreeDPrintProjectTracker.Engine.Interfaces
{
    public interface IPrintTimeEstimationService
    {
        Task<TimeSpan> EstimatePrintTimeAsync(PrintModel model, PrinterProfile profile);
    }
}
