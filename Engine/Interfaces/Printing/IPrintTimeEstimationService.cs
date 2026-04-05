using System;
using System.Threading.Tasks;
using ThreeDPrintProjectTracker.Engine.Models.Printing;

namespace ThreeDPrintProjectTracker.Engine.Interfaces.Printing
{
    public interface IPrintTimeEstimationService
    {
        Task<TimeSpan> EstimatePrintTimeAsync(PrintModel model, PrinterProfile profile);
    }
}
